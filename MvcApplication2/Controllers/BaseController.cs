using DataService;
using MvcApplication2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;


namespace MvcApplication2.Controllers
{
    public class BaseController : Controller
    {
        public PEntities db = new PEntities();


        protected HttpContext ctx
        {
            get { return System.Web.HttpContext.Current; }

        }
        public Employee currentUser
        {
            set { System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] = value; }
            get { return System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee; }
        }

        public Employee Impersonator
        {
            set
            {
                if (value != null)
                    System.Web.HttpContext.Current.Session["impersonator"] = value;
                else
                    System.Web.HttpContext.Current.Session.Remove("impersonator");
            }
            get
            {
                if (System.Web.HttpContext.Current.Session["impersonator"] == null)
                    return null;
                else
                    return System.Web.HttpContext.Current.Session["impersonator"] as Employee;
            }
        }
        public int OrgId
        {
            get
            {
                try
                {
                    if ((int)System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] == 0 && currentUser!=null)
                    {
                        System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] = currentUser.OrgId;
                    }
                    return (int)System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID];
                }
                catch(Exception x)
                {

                    logger.Error(string.Format("Error getting orgid because HttpContext.Current = {0} and session= {1} and value= {2}", System.Web.HttpContext.Current, System.Web.HttpContext.Current.Session, System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID]));
                    if (System.Web.HttpContext.Current!=null && 
                        System.Web.HttpContext.Current.Session!=null && 
                        (int)System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] == 0 && currentUser != null)
                    {
                        System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] = currentUser.OrgId;
                        return (int)System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID];
                    }
                    throw x;

                }
            }
            set { System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] = value; }
        }
        public string AppName
        {
            get { return System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] as string; }
            set { System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] = value; }
        }

        public EvaluationCycle PrevEvalCycle
        {
            get
            {
                Employee CurrentUser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
                EmployeeService e = new EmployeeService((int)Session[CONSTANTS.SESSION_ORG_ID], System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] as string);
                try
                {
                    EvaluationCycle prev = db.EvaluationCycles.Where(x => x.OrgId == OrgId).OrderByDescending(y => y.EvaluationEnd).Skip(1).Take(1).FirstOrDefault();
                    return prev;
                }
                catch
                {
                    return null;
                }
            }
        }

        public EvaluationCycleExtended CurrentEvalCycle
        {
            get
            {
                Employee CurrentUser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
                Hashtable ht = new EmployeeService((int)Session[CONSTANTS.SESSION_ORG_ID], System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] as string).GetPECycleStatus(CurrentUser.gid);
                if (ht.ContainsKey(CONSTANTS.EVALUATION_CYCLE))
                {
                    return (EvaluationCycleExtended)ht[CONSTANTS.EVALUATION_CYCLE];
                }
                else return null;
            }
        }
        public EvaluationCycleExtended CurrentGoalCycle
        {
            get
            {
                Employee CurrentUser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
                Hashtable ht = new EmployeeService((int)Session[CONSTANTS.SESSION_ORG_ID], System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] as string).GetPECycleStatus(CurrentUser.gid);
                if (ht.ContainsKey(CONSTANTS.GOAL_SETTING_CYCLE))
                {
                    return (EvaluationCycleExtended)ht[CONSTANTS.GOAL_SETTING_CYCLE];
                }
                else return null;
            }
        }
        protected readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsOffline
        {
            get
            {
                ViewBag.RemainingTime = null;
                ViewBag.OnlineDate = null;
                if (ctx.Application[CONSTANTS.SHUTDOWN_STARTTIME] != null)
                {
                    DateTime start = (DateTime)ctx.Application[CONSTANTS.SHUTDOWN_STARTTIME];
                    DateTime end = (DateTime)ctx.Application[CONSTANTS.SHUTDOWN_ENDTIME];
                    double remaningTime = start.Subtract(DateTime.Now).TotalMinutes;

                    if (remaningTime < 0)
                    {
                        if (end.Subtract(DateTime.Now).TotalMinutes > 0) // Time to bring down the site
                        {
                            ViewBag.RemainingTime = end.Subtract(DateTime.Now);
                            ViewBag.OnlineDate = end;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        //public EvaluationCycle CurrentPECycle
        //{
        //    get
        //    {

        //        return  new BaseModel(OrgId, AppName).GetCurrentCycle(0);
        //    }
        //}
        //
        // GET: /Base/

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            base.OnActionExecuted(filterContext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                postlogin();
                if (currentUser != null && currentUser.Manager == null && currentUser.gid != null && currentUser.gid.ToString().ToLower() != new Guid().ToString().ToLower())
                {
                    currentUser = db.Employees.FirstOrDefault(x => x.gid == currentUser.gid);
                    System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] = currentUser;
                }
                if (System.Web.HttpContext.Current.Session["hacker"] != null)
                {
                    logger.Error(currentUser.FirstName + " " + currentUser.LastName + " has been detected as a potential hacker.");
                    Mail mail = new Mail();
                    mail.SendEmail("Hacker Detected!", currentUser.FirstName + " " + currentUser.LastName + " has been detected as a potential hacker.");
                    System.Web.HttpContext.Current.Session.Remove("hacker");
                }
            }
            catch { }
            ViewBag.ShutdownMessage = null;
            ViewBag.ShutDownStart = null;
            if (ctx.Application[CONSTANTS.SHUTDOWN_STARTTIME] != null)
            {
                DateTime start = (DateTime)ctx.Application[CONSTANTS.SHUTDOWN_STARTTIME];
                DateTime end = (DateTime)ctx.Application[CONSTANTS.SHUTDOWN_ENDTIME];
                double remaningTime = start.Subtract(DateTime.Now).TotalMinutes;
                if (remaningTime <= 15 && remaningTime > 0)
                {
                    string timeframe = Math.Round(start.Subtract(DateTime.Now).TotalMinutes).ToString() + " mins";
                    if (Math.Round(start.Subtract(DateTime.Now).TotalMinutes) == 0)
                    {
                        timeframe = start.Subtract(DateTime.Now).Seconds.ToString() + " seconds";
                    }
                    ViewBag.ShutdownMessage = string.Format("Velocity will undergo scheduled maintenance for {0} mins. You have <b style='color:red;margin:3px;padding:3px;text-decoration:underline;'>{1} </b> to save your work and logoff.", Math.Round(end.Subtract(start).TotalMinutes).ToString(), timeframe);
                    ViewBag.ShutDownStart = start;
                }
            }
            if (!filterContext.IsChildAction)
            {
                //if (currentUser==null && !string.IsNullOrWhiteSpace(filterContext.RequestContext.HttpContext.User.Identity.Name))
                //{
                //    Session.Clear();
                //    Session.Abandon();
                //    FormsAuthentication.SignOut();
                //}
            }

            base.OnActionExecuting(filterContext);
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewBag.EntityName = RouteData.Values["controller"].ToString().LegitimizeString();
            ViewBag.Me = currentUser;
            try
            {
                ViewBag.OrgId = OrgId;
            }
            catch { }
            if ((filterContext.Result is System.Web.Mvc.ViewResultBase))
            {
                string view = ((System.Web.Mvc.ViewResultBase)(filterContext.Result)).ViewName;
                if (IsOffline && !filterContext.Controller.ControllerContext.IsChildAction && !(filterContext.Controller.ToString().ToLower().EndsWith("homecontroller") && view.ToLower() == "offline"))
                {

                    RedirectResult redirectResult = new RedirectResult("~/home/offline");
                    filterContext.Result = redirectResult;
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                    filterContext.Cancel = true;
                }
            }
            base.OnResultExecuting(filterContext);
        }

        protected bool isEditingAllowed(long evalcycleid, Guid empid, bool isManager)
        {
            bool allowEdit = false;

            if (CurrentGoalCycle != null && evalcycleid == CurrentGoalCycle.Id)
            {

            }
            if (CurrentEvalCycle != null && evalcycleid == CurrentEvalCycle.Id)
            {

            }
            return allowEdit;
        }
        private void postlogin()
        {
            string appname = ConfigurationManager.AppSettings["appname"];
            string notificationStatus = ConfigurationManager.AppSettings["notification"];
            System.Web.HttpContext.Current.Session[CONSTANTS.NOTIFICATION_STATUS] = notificationStatus;
            System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] = appname;

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated == true && Session[CONSTANTS.SESSION_CURRENT_USER] == null)
            {
                Models.ActiveDirectoryHelper admodel = new Models.ActiveDirectoryHelper();


                Employee CurrentUser = null;
                try
                {
                    CurrentUser = admodel.GetUserDetails();
                }
                catch { }
                logger.Info(CurrentUser.FirstName + " " + CurrentUser.LastName + " logged in. And login name is " + User.Identity.Name);
                if (!User.IsInRole("HrAdmin") && User.Identity.Name.ToLower().Contains("nitasha"))
                {
                    try
                    {
                        logger.Info("Adding " + User.Identity.Name + " to HrAdmin Role");
                        Roles.AddUserToRole(User.Identity.Name, "HrAdmin");
                    }
                    catch { }
                }
                if (string.Compare(CurrentUser.Department, "hr", true) == 0 || (!User.IsInRole("Hr") && User.Identity.Name.ToLower().Contains("babita")))
                {
                    logger.Info("Adding " + User.Identity.Name + " to Hr Role");
                    try
                    {
                        Roles.AddUserToRole(User.Identity.Name, "Hr");
                    }
                    catch { }
                }
                if (System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] == null)
                {

                    System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] = CurrentUser.OrgId;
                }
                //try
                //{
                //    log.Info("Session[CONSTANTS.SESSION_ORG_ID] = " + Session[CONSTANTS.SESSION_ORG_ID]);
                //}
                //catch (Exception x)
                //{
                //    log.Error(" When rgetting SessionORGId : "+ x);
                //}

                // try
                //{
                //log.Info("System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] = " + Session[CONSTANTS.SESSION_ORG_ID]);
                //}
                // catch (Exception x)
                // {
                //     log.Error(" When rgetting APPLICATION_NAME : " + x);
                // }
                IEmployeeService empService = new EmployeeService((int)Session[CONSTANTS.SESSION_ORG_ID], System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] as string);
                string un = User.Identity.Name.ToLower();
                if (un.Contains("@"))
                {
                    un = un.Split(new string[] { "@" }, StringSplitOptions.None)[0];
                }

                var emp = empService.GetEmployeeUsingUsername(un);
                string roleToAdd = "Employee";

                if (emp == null || !emp.UserId.HasValue)
                {


                    emp = CurrentUser;

                    if (string.Compare(CurrentUser.Department, "hr", true) == 0)
                    {
                        roleToAdd = "Hr";
                    }


                    //using (var dbx = new PEntities())
                    //{
                    //    var Role = dbx.Roles.SingleOrDefault(x => x.Role1 == roleToAdd);
                    //    if (Role != null)
                    //        CurrentUser.Role = Role.Id;
                    //}

                    if (!Roles.IsUserInRole(roleToAdd))
                    {
                        try
                        {
                            logger.Info("Adding " + User.Identity.Name + " to " + roleToAdd + " Role");
                            Roles.AddUserToRole(User.Identity.Name, roleToAdd);
                        }
                        catch { }
                    }
                    logger.Info("Creating employee..." + User.Identity.Name);
                    empService.CreateEmployee(emp, User.Identity.Name);
                    logger.Info("Created employee..." + User.Identity.Name);
                    emp = empService.GetEmployeeUsingUsername(un);
                    //Hashtable adReportees = empService.GetReportees(emp.gid);
                    //if (adReportees != null)
                    //    logger.Info("User reportees count : " + adReportees.Count);
                    //try
                    //{
                    //    if (adReportees != null && adReportees.Count > 0)
                    //        empService.SetReportingStructure(CurrentUser.gid, adReportees);
                    //}
                    //catch (Exception x)
                    //{
                    //    LogError(x, "While setting reporting structure : ");
                    //}
                }//if emp==null or employee does not exist

                Session[CONSTANTS.SESSION_CURRENT_USER] = emp;
                Session[CONSTANTS.SESSION_ORG_ID] = emp.OrgId;
                var temp = empService.GetReporteesOf(emp.gid);
                Session[CONSTANTS.HAS_A_TEAM] = temp != null && temp.Count > 0;
                var temp2 = empService.GetReviewies(emp.gid);
                Session[CONSTANTS.HAS_REVIEWIES] = temp2 != null && temp2.Count > 0;

                try
                {
                    GoalCycleStatus goalStatus = GoalCycleStatus.NO_ACTION_REQUIRED;
                    EvalCycleStatus evalcyclestatus = EvalCycleStatus.NO_ACTION_REQUIRED;
                    Hashtable ht = empService.GetPECycleStatus(CurrentUser.gid);
                    if (ht != null && ht.Count > 0)
                    {
                        if (ht.ContainsKey(CONSTANTS.GOAL_SETTING_CYCLE))
                        {
                            EvaluationCycleExtended goalcycle = (EvaluationCycleExtended)ht[CONSTANTS.GOAL_SETTING_CYCLE];
                            Session[CONSTANTS.SESSION_CURRENT_GOAL_CYCLE] = goalcycle;
                            goalStatus = goalcycle.GoalSettingStatus;
                        }
                        if (ht.ContainsKey(CONSTANTS.EVALUATION_CYCLE))
                        {
                            EvaluationCycleExtended evalcycle = (EvaluationCycleExtended)ht[CONSTANTS.EVALUATION_CYCLE];
                            Session[CONSTANTS.SESSION_CURRENT_EVALUATION_CYCLE] = evalcycle;
                            evalcyclestatus = evalcycle.EvaluationStatus;
                        }

                    }
                    Session[CONSTANTS.SESSION_CURRENT_GOALSETTING_STATUS] = goalStatus;
                    Session[CONSTANTS.SESSION_CURRENT_EVALUATION_STATUS] = evalcyclestatus;
                }
                catch (Exception x)
                {
                    LogError(x, "While setting getting PE Cycle status : ");

                }

            }// User!=null or user is signed in
        }
        public void LogError(Exception x, string message = "")
        {
            try
            {
                string e = DateTime.Now.ToLongDateString() + " - " + message;
                if (x != null)
                {
                    e += "***Exception Message : " + x.Message;
                    if (x.InnerException != null)
                    {
                        e += "*** Inner Exception : " + x.InnerException.Message + " **Inner stack trace **" + x.InnerException.StackTrace;
                    }
                    e += "***Outer exception : *** " + x.StackTrace + "  *** SOURCE :" + x.Source;

                }
                e += "***Relevant data *** : " + currentUser != null ? currentUser.FirstName + " " + currentUser.LastName : "";
                logger.Error(e);
            }
            catch { }
        }

    }


}