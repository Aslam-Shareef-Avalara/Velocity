using DataService;
using MvcApplication2.Models;
using MvcApplication2.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity.Validation;
namespace MvcApplication2.Controllers
{
    [Table(Name = "C01PR0100")]
    public class C01PR0100
    {
        [Column(IsPrimaryKey = true)]
        public string EmpNo;
        [Column]
        public string EmpFirstName;
        [Column]
        public string EmpLastName;
        [Column]
        public string Sex;
        [Column]
        public string BirthDate;
        [Column]
        public string JoinedOn;
        [Column]
        public string InActiveFrom;
        [Column]
        public string Email;
        [Column]
        public string ReportTo;
        [Column]
        public string Designation;
        [Column]
        public string CostCode;
        [Column]
        public short Status;
    }
    public class PEStatistics
    {
        public int Count { get; set; }
        public string GoalStatus { get; set; }
    }
    [AuthLog(Roles = "HrAdmin, Admin, Hr, Employee", RedirectAction = "YouAreSoScrewed", RedirectController = "Error")]
    public class ControlCenterController : BaseController
    {
        IEmployeeService employeeService;
        IHrEvaluationService hrService;

        public void ScheduleShutdown(string shutdownStartTime, int? durationInMins)
        {
            if (!durationInMins.HasValue || durationInMins < 5)
            {
                durationInMins = 5;
            }
            DateTime startTime = new DateTime();
            ctx.Application[CONSTANTS.SHUTDOWN_STARTTIME] = startTime = DateTime.Parse(shutdownStartTime, CultureInfo.GetCultureInfo("en-US"));
            ctx.Application[CONSTANTS.SHUTDOWN_ENDTIME] = startTime.AddMinutes(durationInMins.Value);
        }

        public ActionResult NotificationDash()
        {
            var enumList = Enum.GetNames(typeof(Mail.ACTION_TYPE));
            var LatestNotifications = db.BulkNotificationLogs.GroupBy(g => new { Reminder = g.ReminderType }).Select(g => new NotificationDashViewModel { ReminderType = g.Key.Reminder, LastRun = g.Max(x => x.SentOn), ReminderTypeDisplay = g.Key.Reminder, Since = (int)DbFunctions.DiffDays(g.Max(x => x.SentOn), DateTime.Today) }).ToList();
            List<NotificationDashViewModel> notfound = new List<NotificationDashViewModel>();
            foreach (var enumName in enumList)
            {
                if (!LatestNotifications.Any(x => x.ReminderType == enumName))
                    notfound.Add(new NotificationDashViewModel { LastRun = null, ReminderTypeDisplay = enumName, ReminderType = enumName, Since = 0 });
            }
            LatestNotifications.AddRange(notfound);
            foreach (var n in LatestNotifications)
            {
                if (n.LastRun.HasValue)
                {
                    var notification = db.BulkNotificationLogs.FirstOrDefault(x => x.ReminderType == n.ReminderType && x.SentOn == n.LastRun);
                    n.NotificationCount = notification.NumberOfNotifications;
                }

            }
            return PartialView(LatestNotifications);
        }

        public ActionResult sync()
        {


            string sql = "SELECT Em.EmpNo, Em.EmpFirstName, EM.EmpLastName, EM.Sex,EM.BirthDate,EM.JoinedOn,EM.InActiveFrom,EM.Email,EM.ReportTo,EM.Designation,EM.CostCode From C01PR0100 EM";
            DataContext dc = new DataContext(ConfigurationManager.ConnectionStrings["ascent"].ConnectionString);
            Table<C01PR0100> ascentData = dc.GetTable<C01PR0100>();
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ascent"].ConnectionString);
            //conn.Open();
            //SqlCommand command = new SqlCommand(sql, conn);
            //SqlDataReader reader =  command.ExecuteReader();
            //var ascentData = db.Database.SqlQuery<AscentBridge>(sql);s
            var temp = ascentData.Where(x => (x.InActiveFrom == null || x.InActiveFrom == "" ) && x.Status==0 && x.InActiveFrom.Length == 0).ToList();
            foreach (var ascentRec in temp)
            {
                try
                {
                    string s = ascentRec.EmpNo.TrimStart(new char[] { '0' });
                    Employee emp = db.Employees.FirstOrDefault(x => x.OrgEmpId == s || x.Email.ToLower().Trim() == ascentRec.Email.ToLower().Trim());
                    if (emp == null)
                    {
                        Employee employee = new Employee();
                        try
                        {
                            try
                            {
                                employee = new ActiveDirectoryHelper().GetUserDetails(ascentRec.Email.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                                if (employee == null) continue;
                            }
                            catch
                            {
                                continue;
                            }

                            employee.Active = true;
                            employee.FirstTime = true;
                            employee.OrgId = OrgId;
                            employee.FirstName = ascentRec.EmpFirstName;
                            employee.LastName = ascentRec.EmpLastName;
                            employee.Gender = ascentRec.Sex;
                            employee.Email = ascentRec.Email;
                            employee.OrgEmpId = int.Parse(ascentRec.EmpNo).ToString();
                            db.Employees.Add(employee);
                            db.SaveChanges();
                        }
                        catch (Exception x)
                        {
                            logger.Error("Could not get new user details from AD or could not save for " + ascentRec.EmpFirstName + " " + ascentRec.EmpLastName + " because " + x.Message, x);
                            continue;
                        }
                        emp = db.Employees.FirstOrDefault(x => x.OrgEmpId == s);
                    }
                    emp.FirstName = ascentRec.EmpFirstName;
                    emp.LastName = ascentRec.EmpLastName;
                    emp.Gender = ascentRec.Sex;
                    try
                    {
                        emp.DoB = DateTime.ParseExact(ascentRec.BirthDate, "yyyyMMdd", CultureInfo.CurrentCulture);
                        }
                    catch(Exception x) {
                        LogError(x, "While parsing DoB = " + x.Message);
                    }//ignore
                    try{
                        emp.DoJ = DateTime.ParseExact(ascentRec.JoinedOn, "yyyyMMdd", CultureInfo.CurrentCulture);
                    }
                    catch(Exception x) {
                        LogError(x, "While parsing DoJ dates = " + x.Message);
                    }//ignore
                    emp.Active = string.IsNullOrEmpty(ascentRec.InActiveFrom);
                    emp.Email = ascentRec.Email;
                    s = ascentRec.ReportTo.TrimStart(new char[] { '0' });
                    if (db.Employees.Any(x => x.OrgEmpId == s))
                    {
                        emp.Manager = db.Employees.FirstOrDefault(x => x.OrgEmpId == s).gid;
                    }
                    else
                    {
                        if (!emp.Manager.HasValue || emp.Manager.Value == new Guid())
                        {
                            emp.Manager = db.Employees.FirstOrDefault(x => x.Email == "sudhir.singh@avalara.com").gid;
                        }
                    }
                    emp.Designation = ascentRec.Designation;
                    emp.Department = ascentRec.CostCode;
                    emp.FirstTime = true;
                    
                    //TBD: When merging into inv tool, need to move this code to DepartmentId col
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    string validationerror = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        validationerror += "   %%%%  " + string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State) + "   %%%%  ";
                        foreach (var ve in eve.ValidationErrors)
                        {
                           validationerror+= "  **** " + string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName), ve.ErrorMessage) + "  **** "; 
                        }
                    }
                    LogError(e, validationerror);
                }
                catch (Exception x)
                {
                    LogError(x, x.Message);
                }
            }
            temp = ascentData.Where(x => (x.InActiveFrom != null && x.InActiveFrom != "") && x.Status==-1 && x.InActiveFrom.Length > 0).ToList();
            foreach (var ascentRec in temp)
            {
                string s = ascentRec.EmpNo.TrimStart(new char[] { '0' });
                Employee emp = db.Employees.FirstOrDefault(x => x.OrgEmpId == s);
                if (emp != null)
                {
                    emp.Active = false;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("index");
        }

        private bool compareValues(string num1, string num2)
        {
            try
            {
                return int.Parse(num1) == int.Parse(num2);
            }
            catch
            {
                return false;
            }
        }
        private int sendBulkNotifications(Mail.ACTION_TYPE actiontype, int goalStatus)
        {
            int notificatinCount = 0;
            try
            {
                if (db.BulkNotificationLogs.Any(x => x.NumberOfNotifications == -1 && x.ReminderType == actiontype.ToString()))
                    return -1;

                BulkNotificationLog notificationLog = new BulkNotificationLog()
                {
                    NumberOfNotifications = -1,
                    ReminderType = actiontype.ToString(),
                    SentOn = DateTime.Now
                };
                db.BulkNotificationLogs.Add(notificationLog);
                db.SaveChanges();

                //Published has to be dealt separately
                if (goalStatus == GoalStatus.PUBLISHED)
                    goalStatus += 2;

                Mail mail = new Mail();


                IQueryable<Guid?> employees = null;
                long evalcycleid = 0;
                if (actiontype == Mail.ACTION_TYPE.SUBMIT_FEEDBACK)
                {
                    if (CurrentEvalCycle == null)
                    {
                        evalcycleid = PrevEvalCycle.Id;
                        logger.Error("Evaluation cycle not active for sending emails... picking up previous eval cycle.");
                    }
                    else
                        evalcycleid = CurrentEvalCycle.Id;

                    if (db.Goals.Any(x => x.Status == GoalStatus.PUBLISHED && x.OrgId == OrgId && x.Evalcycleid == evalcycleid))
                        employees = db.Goals.Where(x => x.Status == GoalStatus.PUBLISHED && x.OrgId == OrgId && x.Evalcycleid == evalcycleid).Select(f => f.EmployeeId).Distinct();
                    else
                    {
                        logger.Error("No employees have published evaluations yet for pe cycle id " + evalcycleid);
                        return 0;
                    }

                    var feedbackCompletedEmployees = db.FeedbackAnswers.Where(x => x.EvalCycleId == evalcycleid).Select(x => x.EmployeeId);
                    if (feedbackCompletedEmployees != null)
                    {
                        employees = employees.Where(z => z.HasValue && !feedbackCompletedEmployees.Contains(z.Value));
                    }
                }
                else
                {
                    switch (actiontype)
                    {
                        case Mail.ACTION_TYPE.APPROVE_EVALUATIONS:
                        case Mail.ACTION_TYPE.EVALUATE_REPORTEE:
                        case Mail.ACTION_TYPE.EVALUATE_SELF:
                        case Mail.ACTION_TYPE.EVALUATION_REJECTED_BY_HR:
                        case Mail.ACTION_TYPE.EVALUATION_REJECTED_BY_MANAGER:
                        case Mail.ACTION_TYPE.PUBLISH:
                        case Mail.ACTION_TYPE.SEND_EVALUATION_TO_HR:
                        case Mail.ACTION_TYPE.SUBMIT_EVALUATION_TO_MANAGER:
                        case Mail.ACTION_TYPE.SUBMIT_FEEDBACK:
                            evalcycleid = CurrentEvalCycle != null ? CurrentEvalCycle.Id : 0;
                            break;
                        default: evalcycleid = CurrentGoalCycle != null ? CurrentGoalCycle.Id : 0;
                            break;
                    }
                    if (goalStatus != GoalStatus.EMPLOYEE_GOAL_SAVED)
                        employees = db.Goals.Where(x => x.Status < goalStatus && x.Status >= goalStatus - 2 && x.OrgId == OrgId && x.Evalcycleid == evalcycleid).Select(f => f.EmployeeId).Distinct();
                    else
                    {
                        //this is for sendng goal setting reminder to all those employees who have not even saved a single goal yet
                        employees = db.Employees.Where(y => y.Active && db.Goals.Any(x => x.Evalcycleid == evalcycleid && x.EmployeeId == y.gid && x.OrgId == OrgId && x.Fixed == false) == false).Select(f => (Guid?)f.gid).Distinct();
                    }

                }
                if (employees != null && employees.Count() > 0)
                {
                    foreach (Guid g in employees)
                    {
                        notificatinCount++;
                        if (!mail.SendEmail(actiontype, g,g, evalcycleid, true))
                            notificatinCount--;
                    }
                }
                notificationLog = db.BulkNotificationLogs.FirstOrDefault(x => x.Id == notificationLog.Id);
                notificationLog.NumberOfNotifications = notificatinCount;
                db.SaveChanges();
            }
            catch (Exception x)
            {
                logger.Error(x.Message + x.StackTrace, x);
                throw x;
            }
            return notificatinCount;
        }


        public JsonResult SendGoalSettingReminderToEmployee()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.SET_YOUR_GOALS, GoalStatus.EMPLOYEE_GOAL_SAVED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendEvaluationReminderToEmployee()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.EVALUATE_SELF, GoalStatus.EMPLOYEE_EVAL_SAVED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendGoalSubmissionReminderToEmployee()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.SEND_GOALS_FOR_APPROVAL, GoalStatus.EMPLOYEE_GOAL_PUBLISHED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendGoalApprovalReminderToManager()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.APPROVE_GOALS, GoalStatus.MANAGER_GOAL_PUBLISHED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendSelfEvaluationReminder()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.SUBMIT_EVALUATION_TO_MANAGER, GoalStatus.EMPLOYEE_EVAL_PUBLISHED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SendAuditReminder()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.SEND_EVALUATION_TO_HR, GoalStatus.MANAGER_EVAL_PUBLISHED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendFeedbackReminder()
        {
            int retval = sendBulkNotifications(Mail.ACTION_TYPE.SUBMIT_FEEDBACK, GoalStatus.PUBLISHED);
            return Json(new { status = retval.ToString().ToUpper() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult stopimpersonating()
        {
            currentUser = Impersonator;
            var temp = employeeService.GetReporteesOf(currentUser.gid);
            Session[CONSTANTS.HAS_A_TEAM] = temp != null && temp.Count > 0;
            var temp2 = employeeService.GetReviewies(currentUser.gid);
            Session[CONSTANTS.HAS_REVIEWIES] = temp2 != null && temp2.Count > 0;
            Session.Remove("impersonator");
            return Redirect("~/employee/aboutme");
        }
        public ActionResult startimpersonating(Guid? g)
        {
            if (!g.HasValue)
                return new EmptyResult();
            Employee emp = db.Employees.FirstOrDefault(x => x.gid == g.Value);
            if (g.Value == currentUser.Manager)
                return View("Message", (object)"Sorry! You cannot impersonate your manager.");
            if (emp == null)
                return new EmptyResult();
            Impersonator = currentUser;
            currentUser = emp;
            var temp = employeeService.GetReporteesOf(currentUser.gid);
            Session[CONSTANTS.HAS_A_TEAM] = temp != null && temp.Count > 0;
            var temp2 = employeeService.GetReviewies(currentUser.gid);
            Session[CONSTANTS.HAS_REVIEWIES] = temp2 != null && temp2.Count > 0;
            return Redirect("~/employee/aboutme");
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            employeeService = new EmployeeService(OrgId, AppName);
            hrService = new HrEvaluationService(OrgId, AppName);
            base.Initialize(requestContext);
        }

        public ActionResult GetPeProgressCharts(long? evalcycleId, int phase = 0)
        {
            Hashtable viewmodel = new Hashtable();
            long evalcycleid = 0;
            List<PEStatistics> pestatistics = null;
            List<PEStatistics> goalStats = null;
            ViewBag.CurrentEvalCycleId = CurrentEvalCycle != null ? CurrentEvalCycle.Id : 0;
            ViewBag.CurrentGoalCycleId = (CurrentGoalCycle != null ? CurrentGoalCycle.Id : 0);
            if (!evalcycleId.HasValue)
            {
                if (CurrentEvalCycle != null)
                {
                    evalcycleid = CurrentEvalCycle.Id;


                    ViewBag.PEName = "No Evaluation Cycle Found";
                    if (evalcycleid > 0)
                        ViewBag.PEName = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid).Title + " (Evaluation Phase)";

                    ViewBag.CycleId = ViewBag.EvalCycleId = evalcycleid;

                    pestatistics = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + evalcycleid.ToString() + " where gs.Id>4 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
                }


                if (CurrentGoalCycle != null)
                {
                    goalStats = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + CurrentGoalCycle.Id.ToString() + " where gs.Id < 5 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
                    //viewmodel.Add("goalstats", goalStats);
                    ViewBag.GoalCycleName = CurrentGoalCycle.Title + " (Goal Setting Phase)";
                    ViewBag.CycleId = ViewBag.GoalCycleId = CurrentGoalCycle.Id;
                }
            }
            else
            {

                evalcycleid = evalcycleId.Value;

                if (phase == 0)
                {
                    ViewBag.PEName = "No Evaluation Cycle Found";
                    if (evalcycleid > 0)
                        ViewBag.PEName = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid).Title + " (Evaluation Phase)";
                    ViewBag.phase = 0;
                    ViewBag.CycleId = ViewBag.EvalCycleId = evalcycleid;

                    pestatistics = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + evalcycleid.ToString() + " where gs.Id>4 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
                }
                else
                {
                    goalStats = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + evalcycleid.ToString() + " where gs.Id < 5 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
                    ViewBag.phase = 1;
                    ViewBag.GoalCycleName = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid).Title + " (Goal Setting Phase)";
                    ViewBag.CycleId = ViewBag.GoalCycleId = evalcycleid;
                }
            }

            viewmodel.Add("goalstats", goalStats);
            var cycles = db.EvaluationCycles.ToList();
            ViewBag.Cycles = cycles;
            viewmodel.Add("pestatistics", pestatistics);
            return View(viewmodel);
        }
        //
        // GET: /ControlCenter/
        public ActionResult Index()
        {
            //  var employees = new ActiveDirectoryHelper().GetUserReportees();
            var emps = db.Employees.Where(x => x.OrgId == OrgId && x.Active).Select(x => new { Name = x.FirstName + " " + x.LastName, gid = x.gid }).OrderBy(x=>x.Name);
            ViewBag.Employees = new SelectList(emps, "gid", "Name");
            Hashtable viewmodel = new Hashtable();
            long evalcycleid = 0;
            List<PEStatistics> pestatistics = null;
            if (CurrentEvalCycle != null)
            {
                evalcycleid = CurrentEvalCycle.Id;
                ViewBag.phase = 1;

                ViewBag.PEName = "No Evaluation Cycle Found";
                if (evalcycleid > 0)
                    ViewBag.PEName = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid).Title + " (Evaluation Phase)";

                ViewBag.EvalCycleId = evalcycleid;

                pestatistics = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + evalcycleid.ToString() + " where gs.Id>4 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
            }
            List<PEStatistics> goalStats = null;

            if (CurrentGoalCycle != null)
            {
                goalStats = db.Database.SqlQuery<PEStatistics>("select   count(EmployeeId)  as Count , status as GoalStatus from (select employeeid, gs.Status as status ,gs.id from Goals g  right   join goalStatus gs on g.status =gs.id  and Fixed=0 and evalcycleid= " + CurrentGoalCycle.Id.ToString() + " where gs.Id < 5 group by EmployeeId,gs.status, gs.id ) as a  group by status, Id order by Id").ToList();
                viewmodel.Add("goalstats", goalStats);
                ViewBag.GoalCycleName = CurrentGoalCycle.Title + " (Goal Setting Phase)";
                ViewBag.GoalCycleId = CurrentGoalCycle.Id;
            }

            viewmodel.Add("pestatistics", pestatistics);
            return View("index_2", viewmodel);
        }

        public void StartPECycle(long evalcycleid, string pename, string description, string goalstart, string goalend, string evaluationstart, string evaluationend)
        {
            IHrEvaluationService hrService = new HrEvaluationService(OrgId, AppName);
            var e = new EvaluationCycle { OrgId = OrgId, Description = description, EvaluationEnd = DateTime.ParseExact(evaluationend, "MM/dd/yyyy", CultureInfo.InvariantCulture), EvaluationStart = DateTime.ParseExact(evaluationstart, "MM/dd/yyyy", CultureInfo.InvariantCulture), GoalEndDate = DateTime.ParseExact(goalend, "MM/dd/yyyy", CultureInfo.InvariantCulture), GoalStartDate = DateTime.ParseExact(goalstart, "MM/dd/yyyy", CultureInfo.InvariantCulture), Title = pename };
            if (evalcycleid == 0)
                hrService.StartPECycle(e);
            else
            {
                e.Id = evalcycleid;
                hrService.EditPECycle(e);
            }
        }

        public ActionResult Organization()
        {
            Guid OrgTempGuid = Guid.NewGuid();
            List<Employee> org = employeeService.OrgStructure();
            var orgstruct = new[] { new { id = 1, name = "Avalara", parent = 0 } }.ToList();
            int i = 2;
            Hashtable guidIdRelation = new Hashtable();
            Hashtable IdGuidRelation = new Hashtable();
            foreach (Employee e in org)
            {
                guidIdRelation[e.gid] = i;
                IdGuidRelation[i] = e.gid;
                orgstruct.Add(new { id = i, name = e.FirstName + " " + e.LastName, parent = e.Manager.HasValue ? (int)guidIdRelation[e.Manager.Value] : 1 });
                i++;
            }
            ViewBag.GidMap = Newtonsoft.Json.JsonConvert.SerializeObject(IdGuidRelation);
            return PartialView("Organization", Newtonsoft.Json.JsonConvert.SerializeObject(orgstruct));
        }

        public ActionResult CreateFixedGoals()
        {
            ViewBag.FixedGoals = true;
            ViewBag.ActionButtonToShow = "Publish";
            ViewBag.Action = "Create Trait";
            ViewBag.AllowEdit = true;
            //Session.Timeout = -1;
            return PartialView("CreateGoal", new Goal() { Evalcycleid = 1 });
        }

        public ActionResult EditPECycles()
        {
            List<EvaluationCycle> evalcycles = hrService.GetValidPECycles();
            return View(evalcycles);
        }

        public ActionResult getpeeditor(long? evalcycleid)
        {
            EvaluationCycle evalcycle = new EvaluationCycle();

            if (evalcycleid.HasValue && evalcycleid.Value != 0)
            {
                evalcycle = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid);
            }
            evalcycle = evalcycle ?? new EvaluationCycle();
            return PartialView("CreateEditPECycle", evalcycle);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateFixedGoals(Goal goal, bool isPeopleGoal = false)
        {
            ViewBag.FixedGoals = true;
            goal.Fixed = true;
            if (!isPeopleGoal)
            {
                isPeopleGoal = System.Web.HttpContext.Current.Request.Form["isPeopleGoal"] == "on";
            }
            goal.OrgId = OrgId;
            goal.ModifiedDate = DateTime.Now;
            goal.ModifiedBy = currentUser.gid;
            goal.gid = Guid.NewGuid();
            goal.EmployeeId = isPeopleGoal ? (Guid?)new Guid() : null;
            using (var dbx = new PEntities())
            {
                dbx.Goals.Add(goal);
                dbx.SaveChanges();
            }
            return Json(new { message = "Goal Create!", saved = true });
        }

        [HttpPost]
        public JsonResult validatepe()
        {
            DateTime goalstart, goalend, evaluationstart, evaluationend;

            goalstart = DateTime.ParseExact(Request.Form["goalstart"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            goalend = DateTime.ParseExact(Request.Form["goalend"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            evaluationstart = DateTime.ParseExact(Request.Form["evaluationstart"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            evaluationend = DateTime.ParseExact(Request.Form["evaluationend"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
            long evalid = long.Parse(Request.Form["evalcycleid"]);
            int Pecount = 0;
            string Newphase = "", Existingphase = "";


            if (db.EvaluationCycles.Any(x => x.Id != evalid && (x.OrgId == OrgId && x.GoalEndDate == goalend && x.GoalStartDate == goalstart && x.EvaluationEnd == evaluationend && x.EvaluationStart == evaluationstart)))
            {
                Pecount = 1;

            }
            if (db.EvaluationCycles.Any(x => x.Id != evalid && ((x.GoalStartDate <= goalstart && x.GoalEndDate >= goalstart) || (x.GoalStartDate <= goalend && x.GoalEndDate >= goalend))))
            {
                Existingphase = Newphase = "Goal Setting";
                Pecount = -1;
            }
            else if (db.EvaluationCycles.Any(x => x.Id != evalid && ((x.EvaluationStart <= evaluationstart && x.EvaluationEnd >= evaluationstart) || (x.EvaluationStart <= evaluationend && x.EvaluationEnd >= evaluationend))))
            {
                Existingphase = Newphase = "Evaluation";
                Pecount = -1;
            }
            else if (goalstart < goalend && goalstart < evaluationstart && goalstart < evaluationend && goalend < evaluationstart && goalend < evaluationend && evaluationstart < evaluationend)
            { }
            else
            {
                if (Pecount == 0)
                {
                    Pecount = -2;
                    Newphase = "Start and end dates of either the goal or evaluation are not chronological.";
                }
            }

            return Json(new { pecount = Pecount, newphase = Newphase, existingphase = Existingphase });
        }
        public ActionResult ReCalculateRatings()
        {
            ManagerEvaluationService mgrSvc = new ManagerEvaluationService(OrgId, AppName);
            List<Employee> employees = db.Employees.Where(x => x.Active && x.OrgId == OrgId).ToList();
            if (employees != null)
            {
                foreach (Employee emp in employees)
                {
                    mgrSvc.CalculateAvgRating(emp.gid, CurrentEvalCycle.Id);
                }
            }
            return Redirect("~/controlcenter");
        }
        public JsonResult currentcyclestatus()
        {
            EvaluationCycle eCycle = CurrentEvalCycle;
            EvaluationCycle gCycle = CurrentGoalCycle;
            List<dynamic> v = new List<dynamic>();
            string formattedDayDateTime = "({0} days and {1} hr : {2} mins remaining)";
            string formattedDateTime = "({0} hr : {1} mins remaining)";
            if (gCycle != null && gCycle.GoalStartDate <= DateTime.Today && gCycle.GoalEndDate >= DateTime.Today)
            {

                TimeSpan t = (TimeSpan)(gCycle.GoalEndDate - DateTime.Now);
                v.Add(new
                {
                    title = gCycle.Title,
                    description = gCycle.Description,
                    start = gCycle.GoalStartDate,
                    end = gCycle.GoalEndDate,
                    cycletype = gCycle.Title + " - Goal Setting",
                    daysRemaining = (t.Days > 0) ? string.Format(formattedDayDateTime, t.Days, t.Hours, t.Minutes) : string.Format(formattedDateTime, t.Hours, t.Minutes),
                    percentComplete = (int)(100.0F * ((float)((TimeSpan)(DateTime.Today - gCycle.GoalStartDate)).Days / (float)((TimeSpan)(gCycle.GoalEndDate - gCycle.GoalStartDate)).Days))
                });

            }
            if (eCycle != null && eCycle.EvaluationStart <= DateTime.Today && eCycle.EvaluationEnd >= DateTime.Today)
            {
                TimeSpan t = (TimeSpan)(eCycle.EvaluationEnd - DateTime.Now);
                v.Add(new
              {
                  title = eCycle.Title,
                  description = eCycle.Description,
                  start = eCycle.EvaluationStart,
                  end = eCycle.EvaluationEnd,
                  cycletype = eCycle.Title + " - Evaluation Process",
                  daysRemaining = (t.Days > 0) ? string.Format(formattedDayDateTime, t.Days, t.Hours, t.Minutes) : string.Format(formattedDateTime, t.Hours, t.Minutes),
                  percentComplete = (int)(100.0F * ((float)((TimeSpan)(DateTime.Today - eCycle.EvaluationStart)).Days / (float)((TimeSpan)(eCycle.EvaluationEnd - eCycle.EvaluationStart)).Days))
              });

            }
            if (v.Count > 0)
            {
                return Json(v, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<dynamic>(){new
            {
                title = "",
                description = ""
            }}, JsonRequestBehavior.AllowGet);

        }

    }
}