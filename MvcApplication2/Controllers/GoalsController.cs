using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using X.Scaffolding.Core;
using PagedList;
using DataService;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using MvcApplication2.ViewModel;
using MvcApplication2.Models;

namespace MvcApplication2.Controllers
{
    [Authorize]
    public class GoalsController : BaseController
    {
        private PEntities db = new PEntities();
        IEmployeeService employeeService = null;
        private int rejected = 0;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

            employeeService = new EmployeeService(OrgId, AppName,currentUser);
            base.Initialize(requestContext);
        }

        public ActionResult GetFixedGoals()
        {
            GoalService goalService = new GoalService(OrgId, AppName,currentUser);
            ViewBag.EmployeeName = "Common";
            ViewBag.FixedGoals = true;
            ViewBag.ActionButtonToShow = "";
            ViewBag.AllowEdit = true;
            GoalsViewModel viewmodel = new GoalsViewModel();
            var goals = goalService.GetFixedGoals();
            EmployeeService es = new EmployeeService(OrgId, AppName,currentUser);
            var reportees = es.GetReportees(currentUser.gid);
            if ((reportees == null || reportees.Count == 0) && !User.IsInRole("Hr") && !User.IsInRole("HrAdmin"))
            {

                goals.RemoveAll(x => x.EmployeeId.HasValue);
            }
            
            viewmodel.ButtonToShow = "";
            viewmodel.PECycle = new EvaluationCycle() { Id = 1, Title = "Fixed Goals" };
            viewmodel.Editable = true;
            viewmodel.TotalWeightage = viewmodel.Goals.Sum(x => x.Weightage.Value);
            viewmodel.Goals = goals;
            return View("Index", new List<GoalsViewModel> { viewmodel });
        }

        private GoalsViewModel UIHintsForGoalCycle(GoalsViewModel goalsviewmodel, Guid empid, EvaluationCycleExtended ec)
        {
            if (currentUser.gid == empid)
            {
                if (ec.GoalSettingStatus < GoalCycleStatus.PUBLISHED)
                {
                    goalsviewmodel.Editable = goalsviewmodel.AllowAdd = true;
                    goalsviewmodel.ButtonToShow = "Publish"; ;
                }
            }
            else
            {
                if (ec.GoalSettingStatus >= GoalCycleStatus.PUBLISHED && ec.GoalSettingStatus < GoalCycleStatus.GOALS_SET)
                {
                    goalsviewmodel.AllowAdd = goalsviewmodel.Editable = false;//= goalsviewmodel.AllowAdd
                    goalsviewmodel.ButtonToShow = "Approve";
                }
            }
            return goalsviewmodel;
        }

        private string GoalSettingPhaseForManager(int goalstatus, ref GoalsViewModel goalsviewmodel, Guid empid, EvaluationCycleExtended ec)
        {
            string titleSuffix = "";
            var employeeOfInterest = db.Employees.FirstOrDefault(c => c.gid == empid);
            switch (goalstatus)
            {
                case GoalStatus.EMPLOYEE_GOAL_SAVED: titleSuffix += "Waiting for goal submission";
                    goalsviewmodel.Goals = new List<Goal>();
                    break;
                case GoalStatus.EMPLOYEE_GOAL_PUBLISHED:
                case GoalStatus.MANAGER_GOAL_SAVED:
                    titleSuffix += "Need your approval";
                    ec.GoalSettingStatus = GoalCycleStatus.PUBLISHED;
                    goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                    goalsviewmodel.PE_Cycle = PECycle.GOAL_SETTING;
                    if (employeeOfInterest.Manager == currentUser.gid)
                        goalsviewmodel.ButtonToShow = "Approve";
                    else
                        goalsviewmodel.ButtonToShow = "";
                    break;
                case GoalStatus.MANAGER_GOAL_PUBLISHED: titleSuffix += "Goals set";
                   // goalsviewmodel.Goals = new List<Goal>();
                    break;
                //default:
                //    throw new InvalidDataException("Goal status not handled in Goal Cycle.");
            }
            return titleSuffix;
        }

        private string GoalSettingPhaseForSelf(int goalstatus, ref GoalsViewModel goalsviewmodel, Guid empid, EvaluationCycleExtended ec)
        {
            string titleSuffix = "";
            switch (goalstatus)
            {
                case GoalStatus.EMPLOYEE_GOAL_SAVED: titleSuffix += "Submit goals to manager";
                    ec.GoalSettingStatus = GoalCycleStatus.STARTED;
                    goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                    goalsviewmodel.PE_Cycle = PECycle.GOAL_SETTING;
                    goalsviewmodel.ButtonToShow = "Publish";
                    break;
                case GoalStatus.EMPLOYEE_GOAL_PUBLISHED:
                case GoalStatus.MANAGER_GOAL_SAVED:
                    goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                    goalsviewmodel.PE_Cycle = PECycle.GOAL_SETTING;
                    titleSuffix += "Awaiting manager approval";
                    break;
                case GoalStatus.MANAGER_GOAL_PUBLISHED: titleSuffix += "Goals set";
                    break;

            }
            return titleSuffix;
        }

        private string EvaluationPhaseForManager(int goalstatus, ref GoalsViewModel goalsviewmodel, Guid empid, EvaluationCycleExtended ec)
        {
            string titleSuffix = "";
            switch (goalstatus)
            {
                case GoalStatus.EMPLOYEE_EVAL_SAVED: titleSuffix += "Waiting for self-evaluation";
                    //goalsviewmodel.Goals = new List<Goal>();
                    break;
                case GoalStatus.EMPLOYEE_EVAL_PUBLISHED:
                case GoalStatus.MANAGER_EVAL_SAVED:
                    titleSuffix += "Awaiting your evaluation";
                    break;
                case GoalStatus.MANAGER_GOAL_PUBLISHED: titleSuffix += "Evaluation published";

                    break;
                case GoalStatus.HR_APPROVED: titleSuffix += "Final publish and meeting pending";
                    break;

                case GoalStatus.PUBLISHED: titleSuffix += "Evaluation Complete";
                    break;
                //default:
                //    throw new InvalidDataException("Goal status not handled in Evaluation Cycle.");
            }
            return titleSuffix;
        }

        private string EvaluationPhaseForSelf(int goalstatus, ref GoalsViewModel goalsviewmodel, Guid empid, EvaluationCycleExtended ec)
        {
            string titleSuffix = "";
            switch (goalstatus)
            {
                case GoalStatus.EMPLOYEE_EVAL_SAVED: titleSuffix += "Submit self-evaluation manager";
                 //   ec.GoalSettingStatus = GoalCycleStatus.STARTED;
                    //goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                    break;
                case GoalStatus.EMPLOYEE_EVAL_PUBLISHED:
                case GoalStatus.MANAGER_EVAL_SAVED:
                    titleSuffix += "Manager review in progress";
                    break;
                case GoalStatus.MANAGER_EVAL_PUBLISHED: titleSuffix += "HR evaluation in progress";
                    break;
                case GoalStatus.HR_APPROVED: titleSuffix += "Final meeting with Manager pending";
                    break;
                case GoalStatus.PUBLISHED: titleSuffix += "Evaluation Complete";
                    break;
                //default:
                //    throw new InvalidDataException("Goal status not handled in Evaluation Cycle.");
            }
            return titleSuffix;
        }

        private GoalsViewModel GetGoalsFor(Hashtable ht, string typeofevalcycle, Guid empid)
        {
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empid);
            GoalService g = new GoalService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            GoalsViewModel goalsviewmodel = null;
            ht = g.GetPECycleStatus(empid);
            if (ht.ContainsKey(typeofevalcycle))
            {
               
                
                goalsviewmodel = new GoalsViewModel();
                
                EvaluationCycleExtended ec = ((EvaluationCycleExtended)ht[typeofevalcycle]);
                goalsviewmodel.Goals = g.GetGoals(empid, ec.Id).OrderBy(x=>x.Fixed).ThenBy(y=>y.ModifiedDate).ToList();
                goalsviewmodel.TotalWeightage = g.GetSelfGoals(empid, ec.Id).Sum(x => x.Weightage).Value;
                goalsviewmodel.EmployeeId = empid;
                goalsviewmodel.PECycle = ec;
                goalsviewmodel.GoalSettingStatus = ec.GoalSettingStatus;
                goalsviewmodel.EvaluationStatus = ec.EvaluationStatus;
                Goal goal = goalsviewmodel.Goals.FirstOrDefault(x => !x.Fixed);
                string titleSuffix = "";

                // If this is Goal Setting phase
                if (typeofevalcycle == CONSTANTS.GOAL_SETTING_CYCLE && goalsviewmodel.PECycle.GoalStartDate <= DateTime.Today && goalsviewmodel.PECycle.GoalEndDate >= DateTime.Today)
                {
                    goalsviewmodel.PE_Cycle = PECycle.GOAL_SETTING;
                    titleSuffix = "Goal Setting";
                    if (currentUser.gid != empid) // Goal Cycle but Manager
                    {
                        if (goal == null)
                        {
                            goalsviewmodel.CurrentStatus = "Waiting for goal submission";
                        }
                        else
                        {
                            goalsviewmodel.CurrentStatus = GoalSettingPhaseForManager(goal.Status.Value, ref goalsviewmodel, empid, ec);
                        }
                    }
                    else // Goal Cycle AND it is self
                    {
                         if (goal == null)
                        {
                             //Enforce GOAL SAVED to show buttons
                            GoalSettingPhaseForSelf(GoalStatus.EMPLOYEE_GOAL_SAVED, ref goalsviewmodel, empid, ec);
                            goalsviewmodel.CurrentStatus = "Set your goals";
                        }
                         else
                         {
                             goalsviewmodel.CurrentStatus = GoalSettingPhaseForSelf(goal.Status.Value, ref goalsviewmodel, empid, ec);
                         }
                    }
                }
                else // Else if this is Evaluation Phase
                {
                    goalsviewmodel.PE_Cycle = PECycle.EVALUATION;
                    titleSuffix = "Evaluation Cycle";
                    if (currentUser.gid != empid) // Eval Cycle but Manager
                    {
                        if (goal == null)
                        {
                            goalsviewmodel.CurrentStatus = "Waiting for goal submission";
                        }
                        else
                        {
                            goalsviewmodel.CurrentStatus = GoalSettingPhaseForManager(goal.Status.Value, ref goalsviewmodel, empid, ec);
                            if (goalsviewmodel.CurrentStatus == "")
                            {
                                goalsviewmodel.CurrentStatus = EvaluationPhaseForManager(goal.Status.Value, ref goalsviewmodel, empid, ec);
                            }
                            
                        }
                    }
                    else // Eval Cycle AND it is self
                    {
                        if (goal == null)
                        {
                            GoalSettingPhaseForSelf(GoalStatus.EMPLOYEE_GOAL_SAVED, ref goalsviewmodel, empid, ec);
                            goalsviewmodel.CurrentStatus = "Create goals";
                        }
                        else
                        {
                            goalsviewmodel.CurrentStatus = GoalSettingPhaseForSelf(goal.Status.Value, ref goalsviewmodel, empid, ec);
                            if (goalsviewmodel.CurrentStatus == "")
                            {
                                goalsviewmodel.CurrentStatus = EvaluationPhaseForSelf(goal.Status.Value, ref goalsviewmodel, empid, ec);
                            }
                            
                        }
                        
                    }
                    if (CurrentEvalCycle != null)
                        goalsviewmodel.ManagerEvaluations = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetReviews(empid, CurrentEvalCycle.Id);

                }
                goalsviewmodel.PECycle.Title += titleSuffix;
                
            }
           
            return goalsviewmodel;
        }

        private GoalsViewModel GetGoalsFor1(Hashtable ht, string typeofevalcycle, Guid empid)
        {
            GoalService g = new GoalService((int)Session[CONSTANTS.SESSION_ORG_ID], AppName,currentUser);

            GoalsViewModel goalsviewmodel = null;

            if (ht.ContainsKey(typeofevalcycle))
            {
                EvaluationCycleExtended ec = ((EvaluationCycleExtended)ht[typeofevalcycle]);
                goalsviewmodel = new GoalsViewModel();
                goalsviewmodel.Goals = g.GetGoals(empid, ec.Id);
                goalsviewmodel.TotalWeightage = g.GetSelfGoals(empid, ec.Id).Sum(x => x.Weightage).Value;
                goalsviewmodel.EmployeeId = empid;
                goalsviewmodel.PECycle = ec;
                goalsviewmodel.GoalSettingStatus = ec.GoalSettingStatus;
                goalsviewmodel.EvaluationStatus = ec.EvaluationStatus;
                Goal goal = goalsviewmodel.Goals.First(x => !x.Fixed);
                string titleSuffix = "";
                if (typeofevalcycle == CONSTANTS.GOAL_SETTING_CYCLE && goalsviewmodel.PECycle.GoalStartDate <= DateTime.Today && goalsviewmodel.PECycle.GoalEndDate >= DateTime.Today)
                {
                    goalsviewmodel.PE_Cycle = PECycle.GOAL_SETTING;
                    titleSuffix = " (Goal Setting)";

                    goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);

                    if (currentUser.gid != empid)
                    {
                        if ((goal == null || goal.Status < GoalStatus.EMPLOYEE_GOAL_PUBLISHED))
                        {
                            titleSuffix = " (Awaiting goal submission)";
                            goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;
                            goalsviewmodel.Goals = new List<Goal>();
                        }
                    }
                    else
                    {
                        if ((goal != null && goal.Status >= GoalStatus.EMPLOYEE_GOAL_PUBLISHED && goal.Status < GoalStatus.MANAGER_GOAL_PUBLISHED))
                        {
                            titleSuffix = " (Awaiting manager's goal approval)";
                            goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;
                        }
                    }
                }

                if (typeofevalcycle == CONSTANTS.EVALUATION_CYCLE && goalsviewmodel.PECycle.EvaluationStart <= DateTime.Today && goalsviewmodel.PECycle.EvaluationEnd >= DateTime.Today)
                {

                    if (goalsviewmodel == null)
                        goalsviewmodel = new GoalsViewModel();
                    var rejectedgoals = goalsviewmodel.Goals.Where(x => !x.Fixed && (x.Rejected.HasValue && x.Rejected.Value));
                    goalsviewmodel.PE_Cycle = PECycle.EVALUATION;

                    if (currentUser.gid != empid)
                    {
                        // If goals were rejected in Eval phase
                        if (rejectedgoals != null && rejectedgoals.ToList().Count() > 0)
                        {
                           
                            if (goal.Status >= GoalStatus.EMPLOYEE_GOAL_PUBLISHED && goal.Status < GoalStatus.MANAGER_GOAL_PUBLISHED)
                            {
                                goalsviewmodel.PE_Cycle = DataService.PECycle.GOAL_SETTING;
                                goalsviewmodel.ManagerEvaluations = new ManagerEvaluationService(OrgId, AppName,currentUser).GetReviews(empid, CurrentEvalCycle.Id);
                                titleSuffix = " (Rejected - Awaiting Your Approval)";
                                goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                            }
                        }
                        else // Else if regular cycle
                        {
                            if ((goal == null || (goal.Status < GoalStatus.EMPLOYEE_EVAL_PUBLISHED && goal.Status >= GoalStatus.MANAGER_GOAL_PUBLISHED)))
                            {
                                titleSuffix = " (Awaiting self-evaluation submission)";
                                goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;
                                goalsviewmodel.Goals = new List<Goal>();
                            }
                            if (ec.EvaluationStatus >= EvalCycleStatus.PUBLISHED && ec.EvaluationStatus < EvalCycleStatus.MANAGER_APPROVED)
                            {
                                titleSuffix = " (Manager evaluation pending)";
                                //goalsviewmodel.ButtonToShow = "Approve";
                            }
                            else if (ec.EvaluationStatus < EvalCycleStatus.PUBLISHED)
                            {
                                titleSuffix = " (Self-evaluation in progress)";
                            }
                            else
                                titleSuffix = " (HR approval in progress)";
                        }

                        if ((goal == null || goal.Status < GoalStatus.EMPLOYEE_GOAL_PUBLISHED))
                        {
                            titleSuffix = " (Awaiting goal submission)";
                            goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;
                            goalsviewmodel.Goals = new List<Goal>();
                        }

                       
                    }
                    else
                    {
                        if (rejectedgoals != null && rejectedgoals.ToList().Count() > 0)
                        {

                            if (goal.Status <= GoalStatus.EMPLOYEE_GOAL_SAVED)
                            {
                                goalsviewmodel.PE_Cycle = DataService.PECycle.GOAL_SETTING;
                                goalsviewmodel.Editable = goalsviewmodel.AllowAdd = true;
                                goalsviewmodel.ButtonToShow = "Publish";
                                goalsviewmodel.ManagerEvaluations = new ManagerEvaluationService(OrgId, AppName,currentUser).GetReviews(empid, CurrentEvalCycle.Id);
                                titleSuffix = " (Goals rejected by Manager)";
                                ec.GoalSettingStatus = GoalCycleStatus.STARTED;
                            }

                            if (goal.Status >= GoalStatus.EMPLOYEE_GOAL_PUBLISHED && goal.Status < GoalStatus.MANAGER_GOAL_PUBLISHED)
                            {
                                titleSuffix = " (Awaiting manager's goal approval)";
                                goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;
                            }
                            goalsviewmodel = UIHintsForGoalCycle(goalsviewmodel, empid, ec);
                        }
                        else
                        {
                            goalsviewmodel.Editable = goalsviewmodel.AllowAdd = false;


                            if (currentUser.gid == empid)
                            {
                                if (ec.EvaluationStatus < EvalCycleStatus.PUBLISHED)
                                {
                                    titleSuffix = " (Self-evaluation pending)";

                                }
                                else if (ec.EvaluationStatus >= EvalCycleStatus.PUBLISHED && ec.EvaluationStatus < EvalCycleStatus.MANAGER_APPROVED)
                                {
                                    titleSuffix = " (Manager evaluation in progress)";
                                }
                                else
                                    titleSuffix = " (HR approval in progress)";
                            }
                            else
                            {

                            }
                        }
                    }
                    goalsviewmodel.PECycle.Title += titleSuffix;
                }

            }// contains key goal setting



            return goalsviewmodel;


        }
        // GET: /Goals/

        public ActionResult Index(int? page)
        {


            Guid? reporteeId = null;
            Guid? idOfEmployeeForGoals = currentUser.gid;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                reporteeId = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
                idOfEmployeeForGoals = reporteeId;
            }
            if (Session["impersonator"] != null && ((Employee)Session["impersonator"]).Manager.ToString() == reporteeId.ToString())
            {
                return View("Message", (object)"You are not authorized to view your manager's evaulations.");
            }

            ViewBag.ReporteeId = idOfEmployeeForGoals.Value;
            ViewBag.Title = "Goals";
            List<GoalsViewModel> viewmodel = new List<GoalsViewModel>();
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == idOfEmployeeForGoals);
            var reporteeServiceObj = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            Hashtable ht = reporteeServiceObj.GetPECycleStatus(idOfEmployeeForGoals.Value);
            GoalService g = new GoalService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            GoalsViewModel goalsviewmodel = GetGoalsFor(ht, CONSTANTS.GOAL_SETTING_CYCLE, idOfEmployeeForGoals.Value);
            if (goalsviewmodel != null)
                viewmodel.Add(goalsviewmodel);

            goalsviewmodel = GetGoalsFor(ht, CONSTANTS.EVALUATION_CYCLE, idOfEmployeeForGoals.Value);

            if (goalsviewmodel != null)
                viewmodel.Add(goalsviewmodel);

            if (currentUser.gid == idOfEmployeeForGoals)
            {
                ViewBag.EmployeeName = "Your";
            }
            else
            {
                if (goalsviewmodel!=null && goalsviewmodel.EvaluationStatus < EvalCycleStatus.PUBLISHED)
                    goalsviewmodel.EvaluationStatus = EvalCycleStatus.NO_ACTION_REQUIRED;
                var reportee = employeeService.GetEmployee(idOfEmployeeForGoals.Value);
                if (reportee != null)
                    ViewBag.EmployeeName = employeeService.GetEmployee(idOfEmployeeForGoals.Value).FirstName + "'s";
                else
                {
                    if (Request.IsAjaxRequest())
                        return PartialView("Message", (object)"This employee has not yet signed up!");
                    else
                        return View("Message", (object)"This employee has not yet signed up!");
                }
            }

            if (ht.Count > 0)
            {
                var en = ht.Values.GetEnumerator();
                en.MoveNext();
                ViewBag.CurrentCycle = en.Current;
            }
            else
                ViewBag.CurrentCycle = new EvaluationCycle() { Id = 0 };
            
            ViewBag.GoalJson = Newtonsoft.Json.JsonConvert.SerializeObject(new Goal());

            if (Request.IsAjaxRequest())
                return PartialView("Index", viewmodel.Count > 0 ? viewmodel : null);
            else
            {
                return View("Index", viewmodel.Count > 0 ? viewmodel : null);
               // return View(viewmodel.Count > 0 ? "Index" : "Message", viewmodel.Count > 0 ? viewmodel : (object)"There are no goals set or this is not a goal setting/evaluation phase.");
            }
        }





        // GET: /Goals/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = await db.Goals.FindAsync(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }
        public ActionResult CreateGoal()
        {
            ViewBag.Action = "Create";
            long evalcycleid = 0L;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["evalcycleid"]))
            {
                evalcycleid = long.Parse(ctx.Request.QueryString["evalcycleid"]);
            }
            

            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            return PartialView("CreateGoal", new Goal()
            {
                Evalcycleid = evalcycleid,
                Fixed = false,
                EmployeeId = empid,
                Goal1 = "",
                OrgId = OrgId,
                Weightage = 0
            });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateGoal(Goal goal)
        {
            
            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            if (ModelState.IsValid)
            {
                goal.Fixed = false;
                goal.OrgId = OrgId;
                goal.ModifiedDate = DateTime.Now;
                goal.ModifiedBy = currentUser.gid;
                goal.gid = new Guid();
                if (goal.Evalcycleid==0)
                    goal.Evalcycleid = CurrentGoalCycle.Id;
                if (goal.EmployeeId.HasValue && goal.EmployeeId.Value.ToString().ToLower() != new Guid().ToString().ToLower())
                {
                    empid = goal.EmployeeId.Value;
                }
                try
                {
                    CreateOrUpdateGoals(new Goal[] { goal }, GoalSaveAction.CREATE_OR_SAVE, empid);
                }
                catch (Exception x)
                {
                    return Json(new { message = x.Message, saved = false });
                }
            }




            //goal.OrgId = OrgId;
            //goal.ModifiedDate = DateTime.Now;
            //goal.ModifiedBy = currentUser.gid;
            //goal.gid = Guid.NewGuid();
            //using (var dbx = new PEntities())
            //{
            //    dbx.Goals.Add(goal);
            //    dbx.SaveChanges();
            //}
            return Json(new { message = "Goal Create!", saved = true });
        }
        // GET: /Goals/Create
        public ActionResult Create()
        {
            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empid);
            ViewBag.EmployeeId = empid;
            ViewBag.EvalCycleId = new GoalService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetGoalSettingCycle();
            return View();
        }
        public class JsonFilter : ActionFilterAttribute
        {
            public string Parameter { get; set; }
            public Type JsonDataType { get; set; }

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.ContentType.Contains("application/json"))
                {
                    string inputContent;
                    filterContext.HttpContext.Request.InputStream.Position = 0;
                    using (var sr = new StreamReader(filterContext.HttpContext.Request.InputStream))
                    {
                        inputContent = sr.ReadToEnd();
                    }

                    var result = JsonConvert.DeserializeObject(inputContent, JsonDataType);
                    filterContext.ActionParameters[Parameter] = result;
                }
            }
        }
        // POST: /Goals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [JsonFilter(JsonDataType = typeof(Goal[]), Parameter = "goals")]
        [ValidateInput(false)]
        public JsonResult Create(Goal[] goals)
        {
            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            if (ModelState.IsValid)
            {
                //int count = 0;
                //var gs = db.Goals.Where(x => !x.Fixed && x.EmployeeId == currentUser.gid && x.Evalcycleid == CurrentGoalCycle.Id && x.Status.Value == GoalStatus.MANAGER_EVAL_SAVED);
                //if (gs != null)
                //    count = gs.Count();
                //if (count == 0)
                //    new Mail().SendEmail(Mail.ACTION_TYPE.SEND_GOALS_FOR_APPROVAL, currentUser.gid, CurrentGoalCycle.Id);
              
                CreateOrUpdateGoals(goals, GoalSaveAction.CREATE_OR_SAVE, empid);
            }
            return Json(new { message = "Goal Create!", saved = true });

        }
        [HttpPost]
        [JsonFilter(JsonDataType = typeof(Goal[]), Parameter = "goals")]
        public ActionResult Approve(Goal[] goals, long evalcycleid)
        {
            rejected = 0;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["rejected"]))
            {
                rejected = int.Parse(ctx.Request.QueryString["rejected"]);
            }
            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            if (ModelState.IsValid)
            {
                if (goals == null || goals.Count() == 0)
                    goals = new Goal[] { new Goal() { gid = new Guid(), Evalcycleid = evalcycleid, Goal1 = "" } };
                new Mail().SendEmail(Mail.ACTION_TYPE.GOALS_APPROVED, empid,currentUser.gid, evalcycleid);
                CreateOrUpdateGoals(goals, GoalSaveAction.APPROVE, empid);
            }
            return Index(1);

        }

        [HttpPost]
        [JsonFilter(JsonDataType = typeof(Goal[]), Parameter = "goals")]
        public ActionResult Reject(Goal[] goals, string rejectionreason, long evalcycleid)
        {
            rejected = 0;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["rejected"]))
            {
                rejected = int.Parse(ctx.Request.QueryString["rejected"]);
            }
            Guid empid = new Guid();
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            long pecycleid = evalcycleid;
            if (ModelState.IsValid)
            {
                Mail.ACTION_TYPE mailAction = Mail.ACTION_TYPE.GOALS_REJECTED;
                Badge b = new Badge();
                RejectedMessage rejectionMessage = new RejectedMessage();

                if(CurrentEvalCycle!=null && CurrentEvalCycle.Id==pecycleid)
                    mailAction = Mail.ACTION_TYPE.EVALUATION_REJECTED_BY_MANAGER;
                if (pecycleid==0 && rejected == 1)
                    pecycleid = CurrentEvalCycle.Id;
                var publishedGoals = db.Goals.Where(x => x.Evalcycleid == pecycleid && x.EmployeeId == empid && x.Status == GoalStatus.EMPLOYEE_GOAL_PUBLISHED);
                if (publishedGoals != null)
                {
                    foreach (Goal g in publishedGoals.ToList())
                    {
                        if (mailAction== Mail.ACTION_TYPE.GOALS_REJECTED)
                        {
                            g.Status = GoalStatus.EMPLOYEE_GOAL_SAVED;
                            b.BadgeType = BadgeType.GOALS_REJECTED;
                            rejectionMessage.EvaluationPhase = PECycle.GOAL_SETTING.ToString();
                            b.Description = "Your goals have been rejected. Please talk to  " + currentUser.FirstName;
                        }
                        else
                        {
                            g.Status = GoalStatus.EMPLOYEE_EVAL_SAVED;
                            b.BadgeType = BadgeType.EVALUATION_REJECTED;
                            rejectionMessage.EvaluationPhase = PECycle.EVALUATION.ToString();
                            b.Description = "Your self-evaluations have been rejected. Please talk to  " + currentUser.FirstName;
                        }
                            db.Entry(g).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    new Mail().SendEmail(mailAction, empid,currentUser.gid, pecycleid);

                    
                   
                   
                    b.FromBadge = currentUser.gid;
                    b.ToBadge = empid;
                    b.Viewed = false;
                    db.Badges.Add(b);
                    db.SaveChanges();

                   
                    rejectionMessage.CreatedDate = DateTime.Now;
                    rejectionMessage.EmployeeId = empid;
                    rejectionMessage.EvalCycleId = evalcycleid;
                    
                    rejectionMessage.Id = Guid.NewGuid();
                    rejectionMessage.RejectionReason = rejectionreason;
                    db.RejectedMessages.Add(rejectionMessage);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public JsonResult getrejectionmessage(long evalcycleid)
        {
            long?[] a = null;
            if (evalcycleid == 0)
            {
                var c = db.Goals.Where(x => x.EmployeeId == currentUser.gid && x.Status < GoalStatus.MANAGER_GOAL_SAVED);

                if (c != null && c.Any())
                {
                    a = c.Select(x => x.Evalcycleid).Distinct().ToArray();
                }
                else return null;
            }
            else
            {
                a = new long?[1];
                a[0] = evalcycleid;
            }
            var rejectedMessages = db.RejectedMessages.Where(x => x.EmployeeId == currentUser.gid && a.Contains(x.EvalCycleId) && x.EvaluationPhase == PECycle.GOAL_SETTING.ToString()).ToList();
            if (rejectedMessages == null || rejectedMessages.Count == 0)
            {
                return null;
            }
            string rejectionMessages = "<pre>";
            rejectedMessages.ForEach(x => rejectionMessages += x.RejectionReason + "<br/>");
            rejectionMessages += "</pre>";
            return Json((object)rejectionMessages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[JsonFilter(JsonDataType = typeof(Goal[]), Parameter = "goals")]
        public ActionResult Publish(long evalcycleid)
        {
            Guid empid = new Guid();
            rejected = 0;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["rejected"]))
            {
                rejected = int.Parse(ctx.Request.QueryString["rejected"]);
            }
            if (!string.IsNullOrEmpty(ctx.Request.QueryString["reporteeid"]))
            {
                empid = Guid.Parse(ctx.Request.QueryString["reporteeid"]);
            }
            else
                empid = currentUser.gid;
            if (ModelState.IsValid)
            {
                var user = db.Employees.FirstOrDefault(x => x.gid == empid);
                new Mail().SendEmail(Mail.ACTION_TYPE.APPROVE_GOALS,user.Manager.Value, empid, evalcycleid);
                CreateOrUpdateGoals(new Goal[] { new Goal() { gid = new Guid(), Evalcycleid = evalcycleid, Goal1="" } }, GoalSaveAction.PUBLISH, empid);
                PECycle pecyclephase = PECycle.GOAL_SETTING;
                //Commented below because when rejecing we always set goal_setting as cycle phase
                //if (CurrentEvalCycle !=null && CurrentEvalCycle.Id == evalcycleid)
                //    pecyclephase = PECycle.EVALUATION;
                var rejectedMessages = db.RejectedMessages.Where(x => x.EmployeeId == empid && x.EvalCycleId == evalcycleid && x.EvaluationPhase == pecyclephase.ToString()).ToList();
                if (rejectedMessages != null && rejectedMessages.Count > 0)
                {
                    db.RejectedMessages.RemoveRange(rejectedMessages);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
            #region MyRegion
            //ViewBag.EmployeeId = new SelectList(db.Employees, "gid", "FirstName", goal.EmployeeId);
            //ViewBag.Evalcycleid = new SelectList(db.EvaluationCycles, "Id", "Title", goal.Evalcycleid);
            //ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", goal.OrgId);
            //return View(goal); 
            #endregion
        }
        private enum GoalSaveAction
        {
            CREATE_OR_SAVE,
            PUBLISH,
            APPROVE
        }
        private void CreateOrUpdateGoals(Goal[] goals, GoalSaveAction action, Guid? empid)
        {
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empid);
            Guid newguid = new Guid();
            GoalService gs = new GoalService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            long pecycle = 0;
            if (goals != null && goals.Count() > 0)
            {

                pecycle = goals[0].Evalcycleid.HasValue? goals[0].Evalcycleid.Value:0;
            }
            if(pecycle==0)
            {
                if (rejected == 0)
                    pecycle = gs.GetGoalSettingCycle();
                else
                    pecycle = CurrentEvalCycle.Id;
            }
            if (goals != null)
            {
                foreach (Goal goal in goals)
                {
                    if (goal.gid.ToString() == newguid.ToString() && string.IsNullOrEmpty(goal.Goal1.Trim()))
                        continue;

                    goal.EmployeeId = empid;
                    goal.Evalcycleid = pecycle;
                    goal.Fixed = false;
                    goal.ModifiedBy = currentUser.gid;
                    goal.ModifiedDate = DateTime.Now;
                    goal.OrgId = OrgId;
                    goal.Weightage = 0; // was goal.Weightage ... changed to remove Weightages

                    if (goal.gid.ToString() == newguid.ToString())
                    {
                        goal.gid = Guid.NewGuid();
                        gs.SaveGoalAsDraft(goal);
                        continue;
                    }
                    else
                    {
                        db.Entry(goal).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            switch (action)
            {
                case GoalSaveAction.CREATE_OR_SAVE: gs.SaveGoals(empid, pecycle);
                    break;
                case GoalSaveAction.APPROVE: gs.ApproveGoals(empid, currentUser.gid, pecycle);
                    break;
                case GoalSaveAction.PUBLISH: gs.PublishGoalsToManager(empid, pecycle);
                    break;
            }

        }
        // GET: /Goals/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = await db.Goals.FindAsync(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "gid", "FirstName", goal.EmployeeId);
            ViewBag.Evalcycleid = new SelectList(db.EvaluationCycles, "Id", "Title", goal.Evalcycleid);
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", goal.OrgId);
            ViewBag.Action = "Update";
            return PartialView("CreateGoal", goal);
        }

        // POST: /Goals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        //[ValidateInput(false)]
        public JsonResult Edit(Goal goal)
        {
            Goal goalFromDb = db.Goals.SingleOrDefault(x => x.gid == goal.gid);
            if (ModelState.IsValid)
            {
                goalFromDb.Title = goal.Title;
                goalFromDb.Weightage = 0;//was goal.Weightage ... chantged to remove weightages
                goalFromDb.Goal1 = goal.Goal1;
                goalFromDb.ModifiedBy = currentUser.gid;
                goalFromDb.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return Json(new { message = "Goal updated!", saved = true });
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "gid", "FirstName", goal.EmployeeId);
            ViewBag.Evalcycleid = new SelectList(db.EvaluationCycles, "Id", "Title", goal.Evalcycleid);
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", goal.OrgId);
            return Json(new { message = "Error updating goal.", saved = false });
        }

        // GET: /Goals/Delete/5
        [HttpPost]
        public JsonResult Delete(Guid? id)
        {
            if (id == null)
            {
                return Json(new { message = "goalid cannot be null", deleted = false });
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return Json(new { message = "Goal not found!", deleted = false });
            }
            else if (!goal.Fixed)
            {
                var mgrEval = db.ManagerEvaluations.SingleOrDefault(x => x.GoalId == goal.gid);
                var emplEval = db.EmployeeEvaluations.SingleOrDefault(x => x.GoalId == goal.gid );
                if (emplEval != null)
                    db.EmployeeEvaluations.Remove(emplEval);
                if (mgrEval != null)
                    db.ManagerEvaluations.Remove(mgrEval);
            }
            db.Goals.Remove(goal);
            db.SaveChanges();
            return Json(new { message = "Deleted", deleted = true });
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
