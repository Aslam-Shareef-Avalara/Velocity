using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class EmployeeEvaluationService:BaseModel,IEmployeeEvaluationService
    {
        private ManagerEvaluationService mgrEvalService = null;  
        public EmployeeEvaluationService(int orgid, string appname)
            : base(orgid,appname)
        {
            mgrEvalService = new ManagerEvaluationService(orgid, appname);
        }



        public void DraftSelfEvaluation(string comment, Guid goalid, int ratingId, Guid empid, long evalCycleId)
        {
            using (PEntities dbx = new PEntities())
            {
                var goal = dbx.Goals.Where(x => x.gid == goalid).FirstOrDefault();
                if (!goal.Fixed)
                {
                    goal.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                    dbx.SaveChanges();
                }
                var goalEval = dbx.EmployeeEvaluations.Where(x => x.GoalId == goalid && x.EmployeeId == empid && x.EvalCycleId == evalCycleId).FirstOrDefault();
                bool isNew = false;
                if (goalEval == null)
                {
                    if (dbx.Goals.Where(g => g.gid == goalid).FirstOrDefault() != null)
                    {
                        goalEval = new EmployeeEvaluation()
                        {
                            GoalId = goalid,
                            Id = Guid.NewGuid(),
                        };
                        isNew = true;
                    }
                    else throw new Exception("Could not find goal " + goalid);
                }
                else
                {
                    goalEval = dbx.EmployeeEvaluations.Where(x => x.GoalId == goalid && x.EmployeeId == empid && x.EvalCycleId == evalCycleId).FirstOrDefault();
                }
                goalEval.GoalRating = ratingId;
                goalEval.Comment = comment;
                goalEval.ModifiedDate = DateTime.Now;
                goalEval.ModifiedBy = empid;
                goalEval.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                goalEval.EmployeeId = empid;
                goalEval.EvalCycleId = evalCycleId;

                if (isNew)
                    dbx.EmployeeEvaluations.Add(goalEval);

                dbx.SaveChanges();

                

            }
            mgrEvalService.CalculateAvgRating(empid, evalCycleId, goalid);
        }

        public void SaveSelfEvaluation(Guid employeeId, long evalCycleId = 0)
        {
            evalCycleId = GetEvalCycle().Id;
            
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId).ToList();
                foreach (Goal goal in goals)
                {
                    goal.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                    dbx.SaveChanges();
                }
                mgrEvalService.CalculateAvgRating(employeeId);
            }
        }

        public void PublishSelfEvaluation(Guid employeeId, long evalCycleId = 0)
        {
            
            evalCycleId = GetEvalCycle().Id;
            SaveSelfEvaluation(employeeId, evalCycleId);
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId).ToList();
                foreach (Goal goal in goals)
                {
                    goal.Status = GoalStatus.EMPLOYEE_EVAL_PUBLISHED;
                    dbx.SaveChanges();
                }

                var selfevals = dbx.EmployeeEvaluations.Where(x => x.EmployeeId == employeeId && x.EvalCycleId== evalCycleId).ToList();
                foreach (EmployeeEvaluation selfeval in selfevals)
                {
                    selfeval.Status = GoalStatus.EMPLOYEE_EVAL_PUBLISHED;
                    dbx.SaveChanges();
                }
                Employee emp = dbx.Employees.SingleOrDefault(x => x.gid == employeeId);
                Badge badge = new Badge();
                badge.BadgeType = BadgeType.EVALUATION_SUBMITTED;
                badge.Description = "You have pending evaluation for " + emp.FirstName + ". Click <a href='~/evaluation/ReporteeEvaluation?reporteeId=" + emp.gid + "'>here</a>";
                badge.FromBadge = employeeId;
                badge.ToBadge = emp.Manager.Value;
                badge.Viewed = false;
                dbx.Badges.Add(badge);
                dbx.SaveChanges();
                try
                {
                    badge = dbx.Badges.Where(x => x.BadgeType == BadgeType.EVALUATION_REJECTED && x.FromBadge == emp.Manager.Value && x.ToBadge == emp.gid).FirstOrDefault();
                    if (badge != null)
                    {
                        Badges badgesService = new Badges(OrgId, AppName);
                        badgesService.Delete(badge.Id);
                    }
                }
                catch { }
            }
            mgrEvalService.CalculateAvgRating(employeeId);
        }

        public List<EmployeeEvaluation> GetSelfEvaluations(Guid employeeId, long evalCycleId = 0)
        {
            if (evalCycleId==0)
            evalCycleId = GetEvalCycle().Id;
            List<EmployeeEvaluation> returnList = new List<EmployeeEvaluation>();
            using (PEntities dbx = new PEntities())
            {
                 var goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId) || (x.Fixed && x.OrgId==OrgId)).Select(c => c.gid);
                 return dbx.EmployeeEvaluations.Where(x => goals.Contains(x.GoalId) && x.EvalCycleId == evalCycleId && x.EmployeeId==employeeId).ToList();
                //foreach(Guid id in goals)
                //{

                //    returnList.Add(dbx.EmployeeEvaluations.FirstOrDefault(x => x.GoalId == id && x.EvalCycleId == evalCycleId && x.EmployeeId == employeeId));
                //}
                // return returnList; 
            }
        }

        public List<ManagerEvaluation> GetManagersEvaluation(Guid employeeId, long evalCycleId = 0)
        {
            if (evalCycleId==0)
            evalCycleId = GetEvalCycle().Id;
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId || (x.Fixed && x.OrgId == OrgId)).Select(c => c.gid);
                return dbx.ManagerEvaluations.Where(x => goals.Contains(x.GoalId) && x.EvalCycleId == evalCycleId && x.EmployeeId==employeeId).ToList();
            }
        }



        public EvaluationRating GetOverallEvaluationRating(Guid employeeId, long evalCycleId = 0)
        {
            if (evalCycleId==0)
            evalCycleId = GetEvalCycle().Id;
            using (PEntities dbx = new PEntities())
            {
                var overallRating = dbx.EvaluationRatings.SingleOrDefault(x => x.EmpId == employeeId && x.EvalCycleId == evalCycleId);
                return overallRating;
            }

        }





        public bool IsEvaluationComplete(Guid employeeId, long evalCycleId = 0)
        {
            if (evalCycleId == 0)
                if (GetEvalCycle() != null)
                    evalCycleId = GetEvalCycle().Id;
                else
                   return true;

            using (PEntities dbx = new PEntities())
            {
                var goal = dbx.Goals.FirstOrDefault(x => x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId && !x.Fixed);
                return goal != null && goal.Status == GoalStatus.PUBLISHED;
            }
        }


        public List<EvaluationCycle> GetAllCycles(Guid? empid=null)
        {
            List<EvaluationCycle> evalCycles = null;

            using (PEntities dbx = new PEntities())
            {
                if (empid == null || !empid.HasValue)
                {
                    if (dbx.EvaluationCycles.Any(x => x.OrgId == OrgId && x.EvaluationEnd < DateTime.Today))
                        evalCycles = dbx.EvaluationCycles.Where(x => x.OrgId == OrgId && x.EvaluationEnd < DateTime.Today).ToList();
                }
                else
                {
                    if(dbx.Goals.Any(x => x.EmployeeId == empid))
                    {
                        List<long?> evalcycleIds= dbx.Goals.Where(x => x.EmployeeId == empid).Select(y => y.Evalcycleid).Distinct().ToList();
                        evalCycles = dbx.EvaluationCycles.Where(x => evalcycleIds.Contains(x.Id) && x.EvaluationEnd < DateTime.Today).ToList();
                    }
                }
            }
            return evalCycles;
        }
    }
}
