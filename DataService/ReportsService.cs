using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class ReportsService : BaseModel
    {
        public ReportsService(int orgid, string appname)
            : base(orgid, appname)
        {

        }

        public List<PEStatusDashboard> GetPEStatusDashboard(long evalcycleid)
        {
            using (PEntities dbx = new PEntities())
            {
                var pestatuslist =
                         dbx.Goals.
                         Where(x => !x.Fixed && x.Evalcycleid == evalcycleid).
                         Join(
                            dbx.GoalStatus,
                            g => g.Status,
                            gs => gs.Id,
                            (g, gs) => new
                            {
                                Id = gs.Id,
                                Status = gs.Status,
                                EmployeeId = g.EmployeeId
                            }
                         ).
                         Join(         // outer collection
                            dbx.Employees,             // inner collection
                            g => g.EmployeeId,             // outer key selector
                            e => e.gid,     // inner key selector
                            (g, e) => new          // result selector 
                            {
                                empcount = e.gid,
                                status = g.Status,
                                statusid = g.Id
                            }
                        ).GroupBy(x => x.status).
                        Select(d => new PEStatusDashboard
                        {
                            Count = d.Select(x => x.empcount).Distinct().Count(),
                            Status = d.Key,
                            StatusId = d.FirstOrDefault().statusid
                        }
                        ).OrderBy(x => x.StatusId);
                if (pestatuslist != null && pestatuslist.Count()>0)
                    return pestatuslist.ToList();
            }
            return new List<PEStatusDashboard>();
        }

        public List<PeProgressModel> GetPeProgress(long evalcycleid, int page = 1, int pagesize = 10, int status = -1, string search = null)
        {
            if (evalcycleid == 0 && (evalcycleid = base.GetEvalCycle().Id) == 0)
                return new List<PeProgressModel>();
            List<PeProgressModel> model = new List<PeProgressModel>();
            using (PEntities dbx = new PEntities())
            {
                List<Employee> c = null;
                if (string.IsNullOrEmpty(search))
                    c = dbx.Employees.Where(x => x.Active).OrderBy(x => x.FirstName).ToList(); //.Skip((page - 1) * pagesize).Take(pagesize)
                else
                    c = dbx.Employees.Where(x => x.Active && (x.FirstName.Contains(search) || x.LastName.Contains(search))).OrderBy(x => x.FirstName).ToList();//.Skip((page - 1) * pagesize).Take(pagesize)
                foreach (var emp in c)
                {
                    Goal g = new Goal();
                    if (status == -1)
                        g = dbx.Goals.FirstOrDefault(x => x.EmployeeId == emp.gid && !x.Fixed && x.Evalcycleid == evalcycleid);
                    else
                        g = dbx.Goals.FirstOrDefault(x => x.EmployeeId == emp.gid && !x.Fixed && x.Evalcycleid == evalcycleid && x.Status == status);
                    if (g != null)
                    {
                        var evalCycleRatings = dbx.EvaluationRatings.FirstOrDefault(x => x.EmpId == emp.gid && x.EvalCycleId == evalcycleid);
                        PeProgressModel peprogessmodel = new PeProgressModel()
                        {
                            goal = g,
                            Employee = emp
                        };
                        if (evalCycleRatings != null)
                        {
                            if (g.Status < GoalStatus.MANAGER_EVAL_PUBLISHED)
                                evalCycleRatings.ManagerOverllRating = 0;
                            peprogessmodel.EvalRating = evalCycleRatings;
                        }

                        peprogessmodel.Evalcycle = dbx.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid);
                        var mgr = dbx.Employees.FirstOrDefault(x => x.gid == emp.Manager);
                        if (mgr != null)
                        {
                            peprogessmodel.Manager = mgr;
                        }
                        else
                        {
                            mgr = new Employee()
                            {
                                Email = "",
                                FirstName = "N/A",
                                LastName = "",
                                gid = new Guid()
                            };
                        }
                        peprogessmodel.FeedbackSubmitted = dbx.FeedbackAnswers.Any(x => x.EmployeeId == emp.gid && x.EvalCycleId == evalcycleid);
                        peprogessmodel.MeetingSummarySubmitted = dbx.EvaluationConclusions.Any(x => x.employeeid == emp.gid && x.evalcycleid == evalcycleid && (x.meetingsummary!=null && x.meetingsummary.Trim()!=""));
                        peprogessmodel.TrainingNeedsSubmitted= dbx.EvaluationConclusions.Any(x => x.employeeid == emp.gid && x.evalcycleid == evalcycleid && (x.training!=null && x.training.Trim()!=""));
                        peprogessmodel.Feedback = dbx.FeedbackAnswers.Where(x => x.EmployeeId == emp.gid && x.EvalCycleId == evalcycleid).ToList();
                        peprogessmodel.Conclusion = dbx.EvaluationConclusions.FirstOrDefault(x => x.employeeid == emp.gid && x.evalcycleid == evalcycleid);
                        model.Add(peprogessmodel);
                    }
                }
                return model;
            }
        }
    }

    public class PeProgressModel
    {
        public Employee Employee = new Employee();
        public Goal goal;
        public Employee Manager = new Employee();
        public EvaluationCycle Evalcycle = new EvaluationCycle();
        public EvaluationRating EvalRating = new EvaluationRating();
        public bool FeedbackSubmitted = false;
        public bool MeetingSummarySubmitted = false;
        public bool TrainingNeedsSubmitted = false;
        public EvaluationConclusion Conclusion = new EvaluationConclusion();
        public List<FeedbackAnswer> Feedback = new List<FeedbackAnswer>();
        
    }

    
    public class PEStatusDashboard
    {
        public int Count = 0;
        public string Status = "";
        public int StatusId = 0;
    }
}
