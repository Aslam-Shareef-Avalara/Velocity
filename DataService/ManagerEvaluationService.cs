using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class ManagerEvaluationService : BaseModel, IManagerEvaluationService
    {
        public ManagerEvaluationService(int orgid, string appname)
            : base(orgid, appname)
        {

        }
        public void DraftReview(Guid goalId, Guid managerId, string comment, int rating, Guid employeeId, long evalcycleid)
        {
            /*
            using (PEntities dbx = new PEntities())
            { 
        
            }
             */
            using (PEntities dbx = new PEntities())
            {

                bool isNew = false;
                ManagerEvaluation me = new ManagerEvaluation();

                if (dbx.ManagerEvaluations.FirstOrDefault(x => x.GoalId == goalId && x.EmployeeId == employeeId && x.EvalCycleId == evalcycleid) == null)
                {
                    me = new ManagerEvaluation();
                    me.GoalId = goalId;
                    me.Id = Guid.NewGuid();
                    isNew = true;
                   

                }
                else
                    me = dbx.ManagerEvaluations.FirstOrDefault(x => x.GoalId == goalId && x.EmployeeId == employeeId && x.EvalCycleId == evalcycleid);

                me.Comment = comment;
                me.GoalRating = rating;
                me.ManagerId = managerId;
                me.ModifiedBy = managerId;
                me.ModifiedDate = DateTime.Now;
                me.EmployeeId = employeeId;
                me.EvalCycleId = evalcycleid;
                me.Status = GoalStatus.MANAGER_EVAL_SAVED;

                if (isNew)
                    dbx.ManagerEvaluations.Add(me);

                dbx.SaveChanges();

                var goal = dbx.Goals.Where(x => x.gid == goalId && !x.Fixed).FirstOrDefault();
                if (goal != null)
                {
                    goal.Status = GoalStatus.MANAGER_EVAL_SAVED;
                    dbx.SaveChanges();
                }

                CalculateAvgRating(employeeId, evalcycleid, goalId);
            }
        }

        public void SaveReview(Guid employeeId, long evalcycleid)
        {
            using (PEntities dbx = new PEntities())
            {



                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalcycleid).ToList();
                CalculateAvgRating(employeeId);
                foreach (Goal goal in goals)
                {
                    goal.Status = GoalStatus.MANAGER_EVAL_SAVED;
                    dbx.SaveChanges();
                }

            }
        }

        public void PublishReview(Guid employeeId, long evalcycleid)
        {
            SaveReview(employeeId, evalcycleid);
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalcycleid).ToList();
                CalculateAvgRating(employeeId);
                foreach (Goal goal in goals)
                {
                    goal.Status = GoalStatus.MANAGER_EVAL_PUBLISHED;
                    dbx.SaveChanges();
                }
                var empevals = dbx.EmployeeEvaluations.Where(x => x.EmployeeId == employeeId && x.EvalCycleId == evalcycleid).ToList();
                foreach (EmployeeEvaluation empeval in empevals)
                {
                    empeval.Status = GoalStatus.MANAGER_EVAL_PUBLISHED;
                    dbx.SaveChanges();
                }

                
                var HrAdminNames = dbx.aspnet_UsersInRoles_GetUsersInRoles(AppName, "HrAdmin").Select(x => x.ToLower()).ToList();
                var HrAdminIds = dbx.aspnet_Users.Where(x => HrAdminNames.Contains(x.LoweredUserName)).Select(n => n.UserId).ToList();
                List<Employee> hradmins = dbx.Employees.Where(x => x.UserId.HasValue && x.Active && HrAdminIds.Contains(x.UserId.Value)).ToList();
                Employee emp = dbx.Employees.SingleOrDefault(x => x.gid == employeeId);
                Employee manager = dbx.Employees.SingleOrDefault(x => x.gid == emp.Manager.Value);
                try
                {
                    var b = dbx.Badges.FirstOrDefault(x => x.ToBadge == manager.gid && x.BadgeType == BadgeType.EVALUATION_SUBMITTED && x.FromBadge == employeeId);
                    if (b != null)
                    {
                        dbx.Badges.Remove(b);
                        dbx.SaveChanges();
                    }
                }
                catch { }
                foreach (Employee hr in hradmins)
                {
                    Badge badge = new Badge();
                    badge.BadgeType = BadgeType.EMPLOYEE_EVALUATED;
                    badge.Description = "You have pending review approval from " + manager.FirstName + " for " + emp.FirstName;
                    badge.FromBadge = emp.gid;
                    badge.ToBadge = hr.gid;
                    badge.Viewed = false;
                    dbx.Badges.Add(badge);
                    dbx.SaveChanges();
                }
            }
        }


        public List<ManagerEvaluation> GetReviews(Guid employeeId, long evalCycleId)
        {
            List<ManagerEvaluation> employeeReviews = new List<ManagerEvaluation>();
            using (PEntities dbx = new PEntities())
            {



                employeeReviews.AddRange(dbx.ManagerEvaluations.Where(x => x.EmployeeId == employeeId && x.EvalCycleId == evalCycleId));

            }
            return employeeReviews;
        }

        public List<EvaluationRating> GetReviewStatus(Guid? employeeId, long evalCycleId)
        {
            List<EvaluationRating> employeeReviews = new List<EvaluationRating>();
            using (PEntities dbx = new PEntities())
            {
                employeeReviews = dbx.EvaluationRatings.Where(x => x.EmpId == employeeId && x.EvalCycleId == evalCycleId).ToList();
            }
            return employeeReviews;
        }
         
        public void SaveFinalReviewComment(string ReviewComment, Guid employeeId, long evalCycleId = 0)
        {
            evalCycleId = GetEvalCycle().Id;
            using (PEntities dbx = new PEntities())
            {
                var finalReview = dbx.EvaluationRatings.SingleOrDefault(x => x.EvalCycleId == evalCycleId && x.EmpId == employeeId);
                if (finalReview != null)
                {
                    finalReview.OverAllReviewComment = ReviewComment;

                    dbx.SaveChanges();
                }
            }
        }


        public EvaluationRating CalculateAvgRating(Guid? employeeId = null, long evalCycleId = 0, Guid? goalid = null)
        {
            
            //decimal avgSelfRating = 0.0M, avgMgrRating = 0.0M, avgSelfTraitRating = 0.0M, avgMgrTraitRating = 0.0M;

            if (GetEvalCycle() == null)
            {
                if (evalCycleId == 0) throw new Exception("Evaluation phase not active nor evalcycleid passed.");
                using (PEntities dbx = new PEntities())
                {
                    return dbx.EvaluationRatings.SingleOrDefault(x => x.EmpId == employeeId && x.EvalCycleId == evalCycleId);
                }
            }
            else
            {
                if (evalCycleId != 0 && evalCycleId != GetEvalCycle().Id)
                {
                    using (PEntities dbx = new PEntities())
                    {
                        return dbx.EvaluationRatings.SingleOrDefault(x => x.EmpId == employeeId && x.EvalCycleId == evalCycleId);
                    }
                }

                evalCycleId = GetEvalCycle().Id;

            }
            using (PEntities dbx = new PEntities())
            {
                EvaluationRating evalRating = dbx.EvaluationRatings.SingleOrDefault(x => x.EmpId == employeeId && x.EvalCycleId == evalCycleId);

                //var goals = new List<Goal>();

                //if (!employeeId.HasValue && !goalid.HasValue)
                //    throw new ArgumentException("Neither the goalid nor the employee id was specified for computing AvgRating.");

                //if (!employeeId.HasValue && goalid.HasValue)
                //{
                //    goals = dbx.Goals.Where(x => x.gid == goalid).ToList();
                //    employeeId = goals.First().EmployeeId;
                //    goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId) || (x.Fixed && x.OrgId == OrgId)).ToList();
                //}

                //goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId) || (x.Fixed && x.OrgId == OrgId)).ToList();

                //List<Guid> goalsList = goals.Select(x => x.gid).ToList();
                //var empEvals = dbx.EmployeeEvaluations.Where(y => y.EvalCycleId == evalCycleId && y.EmployeeId == employeeId && goalsList.Contains(y.GoalId)).ToList();
                //var mgrEvals = dbx.ManagerEvaluations.Where(y => goalsList.Contains(y.GoalId) && y.EvalCycleId == evalCycleId && y.EmployeeId == employeeId).ToList();
                //int fixedGoalsCount = 0, goalsCount = 0;
                //foreach (Goal goal in goals)
                //{
                //    var employeeEval = empEvals.Where(y => y.GoalId == goal.gid && y.EvalCycleId == evalCycleId && y.EmployeeId == employeeId).FirstOrDefault();
                //    var mgrEval = mgrEvals.Where(y => y.GoalId == goal.gid && y.EvalCycleId == evalCycleId && y.EmployeeId == employeeId).FirstOrDefault();
                //    if (goal.Fixed)
                //    {
                //        fixedGoalsCount++;
                //        if (employeeEval != null)
                //            avgSelfTraitRating += ((employeeEval.GoalRating.HasValue ? (decimal)employeeEval.GoalRating.Value : 0.0M));//(decimal)goal.Weightage.Value / 100.0M) * 
                //        if (mgrEval != null)
                //            avgMgrTraitRating += ((mgrEval.GoalRating.HasValue ? (decimal)mgrEval.GoalRating.Value : 0.0M));//(decimal)goal.Weightage.Value / 100.0M) * 
                //    }
                //    else
                //    {
                //        goalsCount++;
                //        if (employeeEval != null)
                //            avgSelfRating += ((employeeEval.GoalRating.HasValue ? (decimal)employeeEval.GoalRating.Value : 0.0M)); //(decimal)goal.Weightage.Value / 100.0M) *
                //        if (mgrEval != null)
                //            avgMgrRating += ((mgrEval.GoalRating.HasValue ? (decimal)mgrEval.GoalRating.Value : 0.0M)); //(decimal)goal.Weightage.Value / 100.0M) *
                //    }
                //}
                //avgSelfRating = Math.Round(((avgSelfRating / goalsCount) * 0.75M) + ((avgSelfTraitRating / fixedGoalsCount) * 0.25M), 2);
                //avgMgrRating = Math.Round((avgMgrRating / goalsCount * 0.75M) + (avgMgrTraitRating / fixedGoalsCount * 0.25M), 2);



                if (evalRating != null)
                {
                    //evalRating.SelfOverallRating = avgSelfRating;
                    //evalRating.ManagerOverllRating = avgMgrRating;
                    //dbx.SaveChanges();
                }
                else
                {
                    evalRating = new EvaluationRating();
                    evalRating.EvalCycleId = evalCycleId;
                    evalRating.EmpId = employeeId.Value;
                    evalRating.ManagerId = dbx.Employees.SingleOrDefault(x => x.gid == employeeId).Manager;
                    //evalRating.SelfOverallRating = avgSelfRating;
                    //evalRating.ManagerOverllRating = avgMgrRating;
                    dbx.EvaluationRatings.Add(evalRating);
                    dbx.SaveChanges();
                }



                return evalRating;
            }


        }




        public bool RejectEvaluations(int resetStep, Guid? employeeId = null, long evalCycleId = 0)
        {

            evalCycleId = GetEvalCycle().Id;
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId).ToList();
                foreach (Goal g in goals)
                {
                    g.Rejected = true;
                    if (resetStep == 1)
                        g.Status = GoalStatus.EMPLOYEE_GOAL_SAVED;
                    else
                        g.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                    dbx.SaveChanges();
                    var empGoal = dbx.EmployeeEvaluations.Single(x => x.GoalId == g.gid && x.EmployeeId == employeeId && x.EvalCycleId == evalCycleId);
                    if (empGoal != null)
                    {
                        empGoal.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                        dbx.SaveChanges();
                    }

                    var mgrEval = dbx.ManagerEvaluations.Single(x => x.GoalId == g.gid && x.EmployeeId==employeeId && x.EvalCycleId==evalCycleId);
                    if (mgrEval != null)
                    {
                        mgrEval.Status = GoalStatus.MANAGER_EVAL_SAVED;
                        dbx.SaveChanges();
                    }
                    var emp = dbx.Employees.FirstOrDefault(x => x.gid == employeeId);
                    try
                    {
                        dbx.Badges.Add(new Badge()
                        {
                            BadgeType = BadgeType.EVALUATION_REJECTED,
                            Description = "Your self-evaluation  was not accepted by your manager. <a href='~/evaluation/SelfEvaluation'>Click Here</a>",
                            FromBadge = emp.Manager.Value,
                            ToBadge = emp.gid,
                            Viewed = false
                        });
                        dbx.SaveChanges();
                    }
                    catch { }

                    
                    
                }
            }
            return true;
        }


        public bool Publish(Guid employeeId, long evalcycleid)
        {

            using (PEntities dbx = new PEntities())
            {
                 Employee emp = dbx.Employees.SingleOrDefault(x => x.gid == employeeId);
                var goals = dbx.Goals.Where(x => x.EmployeeId == employeeId && x.Evalcycleid == evalcycleid).ToList();
                try
                {
                    CalculateAvgRating(employeeId);
                }
                catch { 
                    
                }

                    foreach (Goal goal in goals)
                {
                    goal.Status = GoalStatus.PUBLISHED;
                    dbx.SaveChanges();
                }
                var empevals = dbx.EmployeeEvaluations.Where(x => x.EmployeeId == employeeId && x.EvalCycleId == evalcycleid).ToList();
                foreach (EmployeeEvaluation empeval in empevals)
                {
                    empeval.Status = GoalStatus.PUBLISHED;
                    dbx.SaveChanges();
                }
                try
                {
                   
                    var badge = dbx.Badges.Where(x => x.BadgeType == BadgeType.EVALUATION_SUBMITTED && x.FromBadge == employeeId && x.ToBadge == emp.Manager).FirstOrDefault();
                    if (badge != null)
                    {
                        Badges badgesService = new Badges(OrgId, AppName);
                        badgesService.Delete(badge.Id);
                    }
                }
                catch 
                { 
                   
                }
                try
                {
                   
                    var badge = dbx.Badges.Where(x => x.BadgeType == BadgeType.EVALUATION_REJECTED && x.FromBadge == null && x.ToBadge == emp.Manager).FirstOrDefault();
                    if (badge != null)
                    {
                        Badges badgesService = new Badges(OrgId, AppName);
                        badgesService.Delete(badge.Id);
                    }
                }
                catch
                {

                }
                try
                {

                    var rejectMsg = dbx.RejectedMessages.Where(x => x.EmployeeId == employeeId && x.ManagerID == emp.Manager.Value && x.EvalCycleId == evalcycleid && x.EvaluationPhase == PECycle.EVALUATION.ToString()).FirstOrDefault();
                    if (rejectMsg != null)
                    {
                        dbx.RejectedMessages.Remove(rejectMsg);
                        dbx.SaveChanges();
                    }
                }
                catch { }
                

                //var HrAdminNames = dbx.aspnet_UsersInRoles_GetUsersInRoles(AppName, "HrAdmin").Select(x => x.ToLower()).ToList();
                //var HrAdminIds = dbx.aspnet_Users.Where(x => HrAdminNames.Contains(x.LoweredUserName)).Select(n => n.UserId).ToList();
                //List<Employee> hradmins = dbx.Employees.Where(x => HrAdminIds.Contains(x.UserId)).ToList();
                //Employee emp = dbx.Employees.SingleOrDefault(x => x.gid == employeeId);
                //Employee manager = dbx.Employees.SingleOrDefault(x => x.gid == emp.Manager.Value);
                //foreach (Employee hr in hradmins)
                //{
                //    Badge badge = new Badge();
                //    badge.BadgeType = BadgeType.EMPLOYEE_EVALUATED;
                //    badge.Description = "Please fill in the evaluation feedback form. <a href='~/evaluation/feedback?eid="+emp.gid+"&evalcycleid="+evalcycleid+"'> Click here</a> to view it.";
                //    badge.FromBadge = null;
                //    badge.ToBadge = emp.gid;
                //    badge.Viewed = false;
                //    dbx.Badges.Add(badge);
                //    dbx.SaveChanges();
                //}
            }

            return true;
        }
    }
}
