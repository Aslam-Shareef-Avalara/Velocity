using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{

    
    public class GoalService:BaseModel,IGoalService
    {
        private int p;

        public GoalService(int orgid, string appname, Employee emp)
            : base(orgid,appname,emp)
        {
            // TODO: Complete member initialization
            
        }

       

        public void PublishGoalsToManager(Guid? employeeId, long evalCycleId = 0)
        {
            var goalcycle = GetGoalCycle();

            if (evalCycleId == 0)
            {
                if (goalcycle != null)
                    evalCycleId = goalcycle.Id;
                else if (employeeId != null && employeeId.HasValue)
                {
                    throw new Exception("Cannot publish employee goal since no goal cycle is defined or active.");
                }
            }

            using (PEntities dbx = new PEntities())
            {
                var goalids = dbx.Goals.Where(x => x.OrgId== OrgId && employeeId != null && employeeId.HasValue? x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId: x.Fixed ).Select(y => y.gid);
                if (goalids != null)
                {
                    foreach (Guid g in goalids.ToList())
                    {
                        var goals = dbx.Goals.SingleOrDefault(x => x.gid == g);
                        goals.Status = GoalStatus.EMPLOYEE_GOAL_PUBLISHED;
                        dbx.SaveChanges();
                        
                    }
                }
                Employee emp = dbx.Employees.SingleOrDefault(x=>x.gid==employeeId);
                Badge badge = new Badge();
                badge.BadgeType = BadgeType.GOALS_PUBLISHED;
                badge.Description = "You have pending goal approval from " + emp.FirstName + ". Click <a href='~/goals?reporteeid=" + emp.gid +"'>here</a>";
                badge.FromBadge = employeeId;
                badge.ToBadge = emp.Manager.Value;
                badge.Viewed = false;
                dbx.Badges.Add(badge);
                dbx.SaveChanges();
            }
        }

        public void SaveGoals(Guid? employeeId, long Evalcycleid = 0)
        {
            var goalcycle = GetGoalCycle();
            if (Evalcycleid == 0)
            {
                if (goalcycle != null)
                    Evalcycleid = goalcycle.Id;
                else if (employeeId != null && employeeId.HasValue)
                {
                    throw new Exception("Cannot save employee goal since no goal cycle is defined or active.");
                }
            }
            using (PEntities dbx = new PEntities())
            {
                var goalids = dbx.Goals.Where(x => x.OrgId == OrgId && employeeId != null && employeeId.HasValue ? x.EmployeeId == employeeId && x.Evalcycleid == Evalcycleid : x.Fixed).Select(y => y.gid);
                if (goalids != null)
                {
                    foreach (Guid g in goalids.ToList())
                    {
                        var goals = dbx.Goals.SingleOrDefault(x => x.gid == g);
                        goals.Status = GoalStatus.EMPLOYEE_GOAL_SAVED;
                        dbx.SaveChanges();
                    }
                }
            }
        }

        public List<Goal> GetGoals(Guid employeeId, long evalCycleId)
        {
            using (PEntities dbx = new PEntities())
            {
                IQueryable<Goal> goals = null;
                if (dbx.Employees.Any(x => x.Manager == employeeId && x.Active))
                    goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId) || (x.Fixed == true && x.OrgId == OrgId ));
                else
                    goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId) || (x.Fixed==true && x.OrgId==OrgId && !x.EmployeeId.HasValue));
                if(goals!=null)
                return goals.ToList();
            }
            return new List<Goal>();
        }

        /// <summary>
        /// This is without the fixed goals
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="evalCycleId"></param>
        /// <returns></returns>
        public List<Goal> GetSelfGoals(Guid employeeId, long evalCycleId)
        {
            using (PEntities dbx = new PEntities())
            {
                var goals = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId && x.OrgId == OrgId));
                if (goals != null)
                    return goals.ToList();
            }
            return new List<Goal>();
        }

        public long GetGoalSettingCycle()
        {
            var goalcycle = GetGoalCycle();
            if (goalcycle != null)
                return goalcycle.Id;

            return 0;
        }


        public void ApproveGoals(Guid? employeeId, Guid ManagerId, long evalCycleId = 0)
        {
            var goalcycle = GetGoalCycle();
            if (evalCycleId == 0)
            {
                if (goalcycle != null)
                    evalCycleId = goalcycle.Id;
                else if (employeeId != null && employeeId.HasValue)
                {
                    throw new Exception("Cannot save employee goal since no goal cycle is defined or active.");
                }
            }
            using (PEntities dbx = new PEntities())
            {
                var goalids = dbx.Goals.Where(x => x.OrgId == OrgId && employeeId != null && employeeId.HasValue ?  x.EmployeeId == employeeId && x.Evalcycleid == evalCycleId : x.Fixed).Select(y => y.gid);
                if (goalids != null)
                {
                    foreach (Guid g in goalids.ToList())
                    {
                        var goal = dbx.Goals.SingleOrDefault(x => x.gid == g);
                        goal.Status = GoalStatus.MANAGER_GOAL_PUBLISHED;
                        dbx.SaveChanges();
                    }
                }
                try
                {
                    var b = dbx.Badges.FirstOrDefault(x =>x.ToBadge==ManagerId && x.BadgeType == BadgeType.GOALS_PUBLISHED && x.FromBadge == employeeId.Value);
                    if (b != null)
                    {
                        dbx.Badges.Remove(b);
                        dbx.SaveChanges();
                    }
                }
                catch { }
            }
        }


        public List<Goal> GetFixedGoals()
        {
            using (PEntities dbx = new PEntities())
            {
                IQueryable<Goal> goalids = null;
                
                goalids = dbx.Goals.Where(x => x.Fixed==true && x.OrgId ==OrgId);
                if (goalids != null)
                {

                    return goalids.ToList();
                }
            }

            return new List<Goal>();
        
        }


        public void SaveGoalAsDraft(Goal g)
        {
            using (PEntities dbx = new PEntities())
            {
                g.Status = GoalStatus.EMPLOYEE_GOAL_SAVED;
                dbx.Goals.Add(g);
                dbx.SaveChanges();
                Guid? ManagerId = dbx.Employees.Where(x => x.gid == g.EmployeeId).Select(y => y.Manager).FirstOrDefault();
                if (!ManagerId.HasValue) throw new Exception("No manager has been assigned yet. Your goals cannot be set at this point. Please speak to the HR.");
                dbx.SetGoal(g.gid, g.ModifiedBy, g.Goal1, g.Fixed, g.OrgId, g.EmployeeId, ManagerId, g.Evalcycleid,0);

            }
        }
    }
}
