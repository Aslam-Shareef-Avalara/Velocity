using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MvcApplication2.ViewModel;


namespace MvcApplication2.Controllers
{
    [AuthLog(Roles = "HrAdmin, Admin, Hr,Employee", RedirectAction = "Index", RedirectController = "Employee")]
    public class ReportsController : BaseController
    {
        private ReportsService reportservice = null;
        private const int pagesize = 25;
        public ReportsController()
        {
            reportservice = new ReportsService(OrgId, AppName,currentUser);
        }
        //
        // GET: /Reports/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult peprogress(long evalcycleid = 0, string search = null, int page = 1, int status = -1, int pageSize = pagesize)
        {
            var evalcycles = new List<EvaluationCycle>();
            if (CurrentEvalCycle != null)
                evalcycles.Add(CurrentEvalCycle);
            if (CurrentGoalCycle != null)
                evalcycles.Add(CurrentGoalCycle);
            ViewBag.EvalCycles = evalcycles;
            if (evalcycleid == 0)
            {
                if (evalcycles.Count > 0)
                {
                    evalcycleid = evalcycles.First().Id;
                }
            }
            ViewBag.CurrentFilter = search;
            if (evalcycleid == 0)
                return View("Message", (object)"No cycle is currently active.");
            ViewBag.EvalCycleId = evalcycleid;
            ViewBag.GoalStatuses = db.GoalStatus.OrderBy(x => x.Id).ToList();
            PeProgressViewModel viewmodel = new PeProgressViewModel();
            int empCount = db.Employees.Where(x => x.Active && x.OrgLocationId==OrgId).ToList().Count;
            viewmodel.PageCount = empCount / 25;
            if (empCount % pagesize > 0)
                viewmodel.PageCount++;
            viewmodel.PageNumber = page;

            if (pageSize == 0)
                viewmodel.peprogressList = reportservice.GetPeProgress(evalcycleid, page, pagesize, status, search).ToPagedList(page, pagesize);
            else
                viewmodel.peprogressList = reportservice.GetPeProgress(evalcycleid, page, pageSize, status, search).ToPagedList(page, pageSize);
            Guid g = Impersonator == null ? currentUser.gid : Impersonator.gid;
            var userFromProgress =  viewmodel.peprogressList.FirstOrDefault(x => x.Employee.gid == g);
            ViewBag.RemoveForUser = "";
            if (userFromProgress != null)
            {
                if (db.Goals.Any(x => x.EmployeeId == g && x.Evalcycleid == evalcycleid && !x.Fixed))
                {
                    if (db.Goals.FirstOrDefault(x => x.EmployeeId == g && x.Evalcycleid == evalcycleid && !x.Fixed).Status < GoalStatus.PUBLISHED)
                    {
                        ViewBag.RemoveForUser = userFromProgress.Employee.gid.ToString();
                    }
                }
            }
            
            return View(viewmodel);
        }

        public ActionResult exportpeprogress(long evalcycleid = 0)
        {
            var evalcycles = new List<EvaluationCycle>();
            if (CurrentEvalCycle != null)
                evalcycles.Add(CurrentEvalCycle);
            if (CurrentGoalCycle != null)
                evalcycles.Add(CurrentGoalCycle);
            ViewBag.EvalCycles = evalcycles;
            
            if (evalcycleid == 0)
            {
                if (evalcycles.Count > 0)
                {
                    evalcycleid = evalcycles.First().Id;
                }
            }
                
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"PeProgress.xls\"");
            if (evalcycleid == 0)
                return View("Message", (object)"No cycle is currently active.");
            ViewBag.EvalCycleId = evalcycleid;
            ViewBag.GoalStatuses = db.GoalStatus.OrderBy(x => x.Id).ToList();
            PeProgressViewModel viewmodel = new PeProgressViewModel();
            int empCount = db.Employees.Where(x => x.Active && x.OrgLocationId== OrgId).ToList().Count;
            viewmodel.PageCount = empCount / 25;
            if (empCount % pagesize > 0)
                viewmodel.PageCount++;
            viewmodel.PageNumber = 1;
            viewmodel.peprogressList = reportservice.GetPeProgress(evalcycleid, 1, empCount, -1, null).ToPagedList(1, empCount);
            ViewBag.FeedbackQuestions = db.FeedbackQuestions.Where(x => x.OrgId == OrgId).ToList();
            return PartialView("peprogressexport", viewmodel);
        }

        
        public ActionResult pedashboardstatus(long evalcycleid = 0)
        {
            if (evalcycleid == 0)
            {
                if (CurrentEvalCycle != null)
                    evalcycleid = CurrentEvalCycle.Id;
                else if (CurrentGoalCycle != null)
                    evalcycleid = CurrentGoalCycle.Id;
                else
                    return new EmptyResult();
            }
            ViewBag.EvalCycleId = evalcycleid;
            return PartialView(reportservice.GetPEStatusDashboard(evalcycleid));
        }
    }
}