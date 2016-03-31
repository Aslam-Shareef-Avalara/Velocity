using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{

    public static class CONSTANTS
    {
        public const string
            GOAL_SETTING_STATUS = "GoalSettingStatus",
            EVALUATION_STATUS = "EvalStatus",
            EVALUATION_CYCLE = "evaluationCycle",
            GOAL_SETTING_CYCLE = "goalcycle",
            SESSION_CURRENT_USER = "currentuser",
            SESSION_ORG_ID = "OrgId",
            SESSION_CURRENT_GOALSETTING_STATUS = "goalsettingstatus",
            SESSION_CURRENT_EVALUATION_STATUS = "evaluationstatus",
            SESSION_CURRENT_EVALUATION_CYCLE = "evaluationcycle",
            SESSION_CURRENT_GOAL_CYCLE = "goalcycle",
            APPLICATION_NAME = "appname",
            HAS_A_TEAM = "hasteam",
            ONLINE_USERS = "OnlineUsers",
            HAS_REVIEWIES ="hasreviewies",
            SHUTDOWN_STARTTIME="shutdownstart",
            SHUTDOWN_ENDTIME="shutdownend",
            NOTIFICATION_STATUS = "off";


    }

    public static class GoalStatus
    {
        public const int
         NOT_STARTED = 0,
         EMPLOYEE_GOAL_SAVED = 1,
         EMPLOYEE_GOAL_PUBLISHED = 2,
         MANAGER_GOAL_SAVED = 3,
         MANAGER_GOAL_PUBLISHED = 4,
         EMPLOYEE_EVAL_SAVED = 5,
         EMPLOYEE_EVAL_PUBLISHED = 6,
         MANAGER_EVAL_SAVED = 7,
         MANAGER_EVAL_PUBLISHED = 8,
         HR_APPROVED = 9,
        PUBLISHED = 10;

        public static string Translate(int? statusvalue)
        {
            switch(statusvalue)
            {
                case NOT_STARTED: return "Not Started";
                case EMPLOYEE_GOAL_SAVED: return "Goals Saved By Employee";
                case EMPLOYEE_GOAL_PUBLISHED: return "Goals Published By Employee";
                case MANAGER_GOAL_SAVED: return "Goals Saved By Manager";
                case MANAGER_GOAL_PUBLISHED: return "Goals Approved by Manager";
                case EMPLOYEE_EVAL_SAVED: return "Evaluation Saved By Employee";
                case EMPLOYEE_EVAL_PUBLISHED: return "Evaluation Published By Employee";
                case MANAGER_EVAL_SAVED: return "Evaluation Saved By Manager";
                case MANAGER_EVAL_PUBLISHED: return "Evaluation Completed By Manager";
                case HR_APPROVED: return "Evaluation Published By HR";
                case PUBLISHED: return "Evaluation Complete";
                default: return "";
            }
        }
        public static string NextStage(int? currentStatus)
        {
            switch (currentStatus)
            {
                case NOT_STARTED: return "Employee Needs To Create Goals";
                case EMPLOYEE_GOAL_SAVED: return "Employee Needs To Send Goals For Approval";
                case EMPLOYEE_GOAL_PUBLISHED:
                case MANAGER_GOAL_SAVED: return "Manager Needs To Approve Goals";
                case MANAGER_GOAL_PUBLISHED: return "Employee Needs To Self-Evaluate";
                case EMPLOYEE_EVAL_SAVED: return "Employee Needs To Publish Self-Evaluation";
                case EMPLOYEE_EVAL_PUBLISHED: return "Manager Needs to Review";
                case MANAGER_EVAL_SAVED: return "Manager Needs To Send Review To HR";
                case MANAGER_EVAL_PUBLISHED: return "HR Should Approve Or Reject Evaluations";
                case HR_APPROVED: return "Manager Needs To Publish Evaluation";
                case PUBLISHED: return "Nothing";
                default: return "";
            }
        }
    }

    public class BaseModel
    {
        public int OrgId = 0;
        public string AppName = "";
        public Guid AppId = new Guid();
        public BaseModel(int orgid, string appname)
        {
            OrgId = orgid;
            AppName = appname;
        }
        public EvaluationCycle GetCycle(long id)
        {
            using (PEntities dbx = new PEntities())
            {
                var evalCycle = dbx.EvaluationCycles.Where(c => c.Id==id).FirstOrDefault();
                if (evalCycle != null)
                {
                    return evalCycle;
                }
            }
            return null;
        }
        public EvaluationCycle GetGoalCycle()
        {
          
                using (PEntities dbx = new PEntities())
                {
                    var evalCycle = dbx.EvaluationCycles.Where(c => c.GoalStartDate <= DateTime.Today && c.GoalEndDate >= DateTime.Today && c.OrgId == OrgId).FirstOrDefault();
                    if (evalCycle != null)
                    {
                        return evalCycle;
                    }
                }
            
            return null;
        }
        public EvaluationCycle GetEvalCycle()
        {
          
                using (PEntities dbx = new PEntities())
                {
                    var evalCycle = dbx.EvaluationCycles.Where(c => c.EvaluationStart <= DateTime.Today && c.EvaluationEnd >= DateTime.Today && c.OrgId == OrgId).FirstOrDefault();
                    if (evalCycle != null)
                    {
                        return evalCycle;
                    }
                }
            
            return null;
        }

        public EvaluationCycle PrevEvalCycle
        {
            get
            {
                try
                {
                    using (PEntities dbx = new PEntities())
                    {
                        EvaluationCycle prev = dbx.EvaluationCycles.Where(x => x.OrgId == OrgId).OrderByDescending(y => y.EvaluationEnd).Skip(1).Take(1).FirstOrDefault();
                        return prev;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<EvaluationCycle> GetCurrentCycle()
        {
            using (PEntities dbx = new PEntities())
            {

                    var evalCycle = dbx.EvaluationCycles.Where(c => c.GoalStartDate <= DateTime.Today && c.EvaluationEnd >= DateTime.Today && c.OrgId == OrgId).ToList();
                    if (evalCycle != null)
                    {
                        return evalCycle;
                    }

                

            }
            return null;
        }


        public Hashtable GetPECycleStatus(Guid empId)
        {
            Hashtable retVal = new Hashtable();
            using (var dbx = new PEntities())
            {
                var emp = dbx.Employees.FirstOrDefault(x=>x.gid==empId);

                var pecycles = new HrEvaluationService(OrgId, AppName).GetActivePECycles().OrderBy(x => x.Id);

                if (pecycles != null)
                {
                    foreach (EvaluationCycle c in pecycles)
                    {
                        var goal = dbx.Goals.Where(x => x.EmployeeId == empId && x.Evalcycleid == c.Id && !x.Fixed).FirstOrDefault();
                        EvaluationCycleExtended evalX = new EvaluationCycleExtended(c);


                        if (c.EvaluationStart > DateTime.Today)
                        {

                            if (goal != null)
                            {
                                switch (goal.Status)
                                {
                                    case GoalStatus.EMPLOYEE_GOAL_SAVED: evalX.GoalSettingStatus = GoalCycleStatus.STARTED;
                                        break;
                                    case GoalStatus.EMPLOYEE_GOAL_PUBLISHED: evalX.GoalSettingStatus = GoalCycleStatus.PUBLISHED;
                                        break;
                                    case GoalStatus.MANAGER_GOAL_SAVED: evalX.GoalSettingStatus = GoalCycleStatus.MANAGER_STARTED;
                                        break;
                                    case GoalStatus.MANAGER_GOAL_PUBLISHED: evalX.GoalSettingStatus = GoalCycleStatus.GOALS_SET;
                                        break;
                                    // This default means that none of the goal cycle status  matched hence we are in the eval cycle..  so set goal cycle to complete
                                    default: evalX.GoalSettingStatus = GoalCycleStatus.GOALS_SET;
                                        break;
                                }
                            }
                            else
                                evalX.GoalSettingStatus = GoalCycleStatus.NOT_STARTED;

                            retVal[CONSTANTS.GOAL_SETTING_CYCLE] = evalX;
                        }
                        else
                        {
                            if (goal != null)
                            {

                                switch (goal.Status)
                                {
                                    case GoalStatus.EMPLOYEE_EVAL_SAVED: evalX.EvaluationStatus = EvalCycleStatus.STARTED;
                                        break;
                                    case GoalStatus.EMPLOYEE_EVAL_PUBLISHED: evalX.EvaluationStatus = EvalCycleStatus.PUBLISHED;
                                        break;
                                    case GoalStatus.MANAGER_EVAL_SAVED: evalX.EvaluationStatus = EvalCycleStatus.MANAGER_STARTED;
                                        break;
                                    case GoalStatus.MANAGER_EVAL_PUBLISHED: evalX.EvaluationStatus = EvalCycleStatus.MANAGER_APPROVED;
                                        break;
                                    case GoalStatus.HR_APPROVED: evalX.EvaluationStatus = EvalCycleStatus.HR_APPROVED;
                                        break;
                                    case GoalStatus.PUBLISHED: evalX.EvaluationStatus = EvalCycleStatus.COMPLETED;
                                        break;
                                    // This default means that none of the eval cycle status  matched hence we are in the goal cycle..  so set eval cycle to not started
                                    default: evalX.EvaluationStatus = EvalCycleStatus.NOT_STARTED;
                                        break;
                                }
                            }
                            else
                                evalX.EvaluationStatus = EvalCycleStatus.NOT_STARTED;

                            retVal[CONSTANTS.EVALUATION_CYCLE] = evalX;
                        }


                        #region commented code
                        //int publishedCount = dbx.EmployeeEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.e.Published && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();
                        //int savedCount = dbx.EmployeeEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.e.Saved.Value && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();

                        //if (publishedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.PUBLISHED;
                        //    retVal[c.Title] = ht;
                        //}
                        //else if (savedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.STARTED;
                        //    retVal[c.Title] = ht;
                        //}

                        //publishedCount = dbx.ManagerEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.e.Published.Value && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();
                        //savedCount = dbx.ManagerEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.e.Saved.Value && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();
                        //if (publishedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.MANAGER_APPROVED;
                        //    retVal[c.Title] = ht;
                        //}
                        //else if (savedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.MANAGER_STARTED;
                        //    retVal[c.Title] = ht;
                        //}

                        //publishedCount = dbx.EvaluationRatings.Join(dbx.Goals, e => e.EmpId, g => g.EmployeeId, (e, g) => new { e, g }).Where(z => z.e.HrApprovalStage2.Value && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();
                        //savedCount = dbx.EvaluationRatings.Join(dbx.Goals, e => e.EmpId, g => g.gid, (e, g) => new { e, g }).Where(z => z.e.HrApprovalStage1.Value && empId == z.g.EmployeeId && z.g.Evalcycleid == c.Id).Count();
                        //if (publishedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.COMPLETED;
                        //    retVal[c.Title] = ht;
                        //}
                        //else if (savedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["EvalCycleStatus"] = EvalCycleStatus.HR_APPROVED;
                        //    retVal[c.Title] = ht;
                        //}
                        //publishedCount = dbx.Goals.Where(x => x.EmployeeId == empId && x.Status == GoalStatus.EMPLOYEE_GOAL_PUBLISHED && x.Evalcycleid == c.Id).Count();
                        //savedCount = dbx.Goals.Where(x => x.EmployeeId == empId && x.Saved.Value && x.Evalcycleid == c.Id).Count();
                        //int approved = dbx.Goals.Where(x => x.EmployeeId == empId && x.Approved.Value && x.Evalcycleid == c.Id).Count();
                        //if (publishedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["GoalCycleStatus"] = GoalCycleStatus.PUBLISHED;
                        //    retVal[c.Title] = ht;
                        //}
                        //else if (savedCount > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["GoalCycleStatus"] = GoalCycleStatus.STARTED;
                        //    retVal[c.Title] = ht;
                        //}
                        //else if (approved > 0)
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["GoalCycleStatus"] = GoalCycleStatus.GOALS_SET;
                        //    retVal[c.Title] = ht;
                        //}
                        //else
                        //{
                        //    ht = retVal[c.Title] as Hashtable;
                        //    ht["GoalCycleStatus"] = GoalCycleStatus.NOT_STARTED;
                        //    retVal[c.Title] = ht;
                        //} 
                        #endregion
                    }
                }
            }
            return retVal;
        }

        public List<Attachment> GetAttachments(Guid evaluationRecordId)
        {
            List<Attachment> attachments = new List<Attachment>();
            using (PEntities dbx = new PEntities())
            {
                attachments = dbx.Attachments.Where(x => x.EvaluationGoalId == evaluationRecordId).ToList();
            }
            return attachments;
        }

        public Guid SaveAttachment(Guid evaluationRecordId, byte[] fileContents, string fileName, string fileType)
        {
            using (PEntities dbx = new PEntities())
            {
                int count = dbx.Attachments.Count(x => x.EvaluationGoalId == evaluationRecordId);
                if (count == 5)
                    throw new InvalidOperationException("Cannot add more than 5 attachments per goal");
                Attachment attachment = new Attachment();
                attachment.AttachmentId = Guid.NewGuid();
                attachment.EvaluationGoalId = evaluationRecordId;
                attachment.EvaluationType = EvaluationType.SELF_EVALUATION;
                attachment.FileContents = fileContents;
                attachment.FileName = fileName;
                attachment.FileType = fileType;
                attachment.FileSize = ((float)fileContents.Length / 1000.0f).ToString();
                dbx.Attachments.Add(attachment);
                dbx.SaveChanges();
                return attachment.AttachmentId;
            }
        }

        public bool DeleteAttachment(Guid attachmentId)
        {
            using (PEntities dbx = new PEntities())
            {
                var attachment = dbx.Attachments.FirstOrDefault(x => x.AttachmentId == attachmentId);
                if (attachment != null)
                {
                    dbx.Attachments.Remove(attachment);
                    dbx.SaveChanges();
                    return true;
                }

            }
            return false;
        }
    }
}
