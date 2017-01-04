
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class HrEvaluationService : BaseModel, IHrEvaluationService
    {

        public HrEvaluationService(int orgid, string appname, Employee emp)
            : base(orgid, appname,emp)
        {

        }

        public List<Employee> GetEmployeesWhoHaveNotStartedEvalCycle(long evalCycleId = 0)
        {
            evalCycleId = GetGoalCycle().Id;

            using (PEntities dbx = new PEntities())
            {


                var t = dbx.EmployeeEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => !(z.g.Status < GoalStatus.EMPLOYEE_EVAL_SAVED) && z.g.Evalcycleid == evalCycleId).Select(x => x.g.EmployeeId);
                var employees = dbx.Employees.Where(x => t.Contains(x.gid) && x.Active);
                if (employees != null)
                    return employees.ToList();


            }
            return null;
        }

        public List<Employee> GetEmployeesWhoHaveNotPublishedComments(long evalCycleId = 0)
        {
            if(evalCycleId<=0)
            evalCycleId = GetEvalCycle().Id;

            using (PEntities dbx = new PEntities())
            {
                var t = dbx.EmployeeEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.g.Status != GoalStatus.EMPLOYEE_EVAL_PUBLISHED && z.g.Evalcycleid == evalCycleId).Select(x => x.g.EmployeeId);
                var employees = dbx.Employees.Where(x => t.Contains(x.gid) && x.Active);
                if (employees != null)
                    return employees.ToList();

            }
            return null;
        }

        public List<Employee> GetManagersWhoHaveNotStartedReviews(long evalCycleId = 0)
        {
            if (evalCycleId <= 0)
            evalCycleId = GetEvalCycle().Id;

            using (PEntities dbx = new PEntities())
            {
                var t = dbx.ManagerEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => !(z.g.Status < GoalStatus.MANAGER_EVAL_SAVED) && z.g.Evalcycleid == evalCycleId).Select(x => x.e.ManagerId);
                var employees = dbx.Employees.Where(x => t.Contains(x.gid));
                if (employees != null)
                    return employees.ToList();
            }
            return null;
        }

        public List<Employee> GetManagersWhoHaveNotPublishedReviews(long evalCycleId = 0)
        {
            if (evalCycleId <= 0)
            evalCycleId = GetEvalCycle().Id;

            using (PEntities dbx = new PEntities())
            {
                var t = dbx.ManagerEvaluations.Join(dbx.Goals, e => e.GoalId, g => g.gid, (e, g) => new { e, g }).Where(z => z.g.Status != GoalStatus.MANAGER_GOAL_PUBLISHED && z.g.Evalcycleid == evalCycleId).Select(x => x.e.ManagerId);
                var employees = dbx.Employees.Where(x => t.Contains(x.gid) && x.Active);
                if (employees != null)
                    return employees.ToList();
            }

            return null;
        }


        public EvaluationCycle StartPECycle(EvaluationCycle evalCycle)
        {
            using (PEntities dbx = new PEntities())
            {
                //TODO: Check if there is an overlap
                var evalcycle = dbx.EvaluationCycles.Add(evalCycle);
                dbx.SaveChanges();
                return evalcycle;
            }
        }


        public List<EvaluationCycle> GetActivePECycles()
        {
            using (PEntities dbx = new PEntities())
            {
                var t = dbx.EvaluationCycles.Where(x => (x.GoalStartDate <= DateTime.Today && x.EvaluationEnd >= DateTime.Today) && x.OrgId == OrgId);

                if (t != null)
                    return t.ToList();
            }
            return null;
        }


        public List<EvaluationCycle> GetValidPECycles()
        {
            using (PEntities dbx = new PEntities())
            {
                IEnumerable<EvaluationCycle> t = null;
                if (CurrentUser == null || !CurrentUser.Department.ToLower().Contains("hr"))
                {
                    t = dbx.EvaluationCycles.Where(x => x.OrgId == OrgId).OrderByDescending(x => x.EvaluationEnd);
                }
                else
                {
                    
                    t = dbx.EvaluationCycles.Join(dbx.OrgLocations, c => c.OrgId, o => o.Id, (c, o) => new {cy=c,og=o }).Where(x => x.og.OrgId == CurrentUser.OrgId).OrderByDescending(x => x.cy.EvaluationEnd).Select(v=>v.cy);
                }
                if (t != null)
                    return t.ToList();
            }
            return null;
        }


        public void PublishEvaluation(Guid empid, long evalCycleId)
        {

            using (PEntities dbx = new PEntities())
            {
                var t = dbx.Goals.Where(x => (x.EmployeeId == empid && x.Evalcycleid == evalCycleId)).ToList();

                if (t != null)
                {
                    Employee emp = dbx.Employees.First(x => x.gid == empid);
                    foreach (Goal g in t)
                    {
                        g.Status = GoalStatus.HR_APPROVED;
                        dbx.SaveChanges();
                    }
                    try
                    {
                        //dbx.Badges.Add(new Badge()
                        //{
                        //    BadgeType = BadgeType.EVALUATION_APPROVED,
                        //    Description = "Your evaluation of " + emp.FirstName + " performance has been approved by HR. To view the evaluations <a href='~/evaluation/ReporteeEvaluation?reporteeId=" + emp.gid + "'>Click Here</a>",
                        //    FromBadge = null,
                        //    ToBadge = emp.Manager.Value,
                        //    Viewed = false
                        //});
                        //dbx.SaveChanges();
                    }
                    catch { }
                }

            }

        }


        public void RejectManagersEvaluation(Guid employeeId, long evalcycleId, string reason)
        {
            using (PEntities dbx = new PEntities())
            {
                var t = dbx.Goals.Where(x => (x.EmployeeId == employeeId && x.Evalcycleid == evalcycleId)).ToList();
                if (t != null)
                {
                    foreach (Goal g in t)
                    {
                        g.Status = GoalStatus.EMPLOYEE_EVAL_PUBLISHED;
                        dbx.SaveChanges();
                    }
                    Goal goal = t.First();
                    Employee emp = dbx.Employees.First(x => x.gid == employeeId);
                    if (emp.Manager != null)
                    {
                        RejectedMessage r = new RejectedMessage()
                        {
                            CreatedDate = DateTime.Now,
                            EmployeeId = employeeId,
                            ManagerID = emp.Manager,
                            EvalCycleId = evalcycleId,
                            EvaluationPhase = PECycle.EVALUATION.ToString(),
                            Id = Guid.NewGuid(),
                            RejectionReason = reason
                        };


                        dbx.RejectedMessages.Add(r);
                        dbx.SaveChanges();
                        try
                        {
                            dbx.Badges.Add(new Badge()
                            {
                                BadgeType = BadgeType.EVALUATION_REJECTED,
                                Description = "Your evaluation of " + emp.FirstName + " performance was not accepted by HR and has been returned back to you. <a href='~/evaluation/ReporteeEvaluation?reporteeId=" + emp.gid + "'>Click Here</a>",
                                FromBadge = null,
                                ToBadge = emp.Manager.Value,
                                Viewed = false
                            });
                            dbx.SaveChanges();
                        }
                        catch { }

                        try
                        {
                            var b = dbx.Badges.Where(x => x.FromBadge ==emp.gid && x.BadgeType == BadgeType.EMPLOYEE_EVALUATED).ToList();
                            if (b != null  && b.Count()>0)
                            {
                                b.ForEach(x => { dbx.Badges.Remove(x); }); 
                                dbx.SaveChanges();
                            }
                        }
                        catch { }
                    }
                }

            }
        }


        public EvaluationCycle EditPECycle(EvaluationCycle evalCycle)
        {
            using (PEntities dbx = new PEntities())
            {
                if (dbx.EvaluationCycles.Any(x => x.Id == evalCycle.Id))
                {
                    var e = dbx.EvaluationCycles.First(x => x.Id == evalCycle.Id);
                    e.Title = evalCycle.Title;
                    e.Description = evalCycle.Description;
                    e.GoalStartDate = evalCycle.GoalStartDate;
                    e.GoalEndDate = evalCycle.GoalEndDate;
                    e.EvaluationStart = evalCycle.EvaluationStart;
                    e.OrgId = evalCycle.OrgId;
                    e.EvaluationEnd = evalCycle.EvaluationEnd;
                    dbx.SaveChanges();
                    return e;
                }
                else
                {
                    return StartPECycle(evalCycle);
                }
            }
        }
    }
}