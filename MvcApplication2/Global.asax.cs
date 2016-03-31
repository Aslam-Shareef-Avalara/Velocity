using DataService;
using MvcApplication2.Controllers;
using MvcApplication2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace MvcApplication2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (!Roles.RoleExists("Employee")) Roles.CreateRole("Employee");
            if (!Roles.RoleExists("Hr")) Roles.CreateRole("Hr");
            if (!Roles.RoleExists("HrAdmin")) Roles.CreateRole("HrAdmin");
            if (!Roles.RoleExists("Admin")) Roles.CreateRole("Admin");



        }
        void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                string ipaddress = "";
                Exception x = Server.GetLastError();
                try
                {
                    ipaddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                catch { }
                if (System.Web.HttpContext.Current.Request == null)
                    LogError(x, "Application Level Error : " + DateTime.Now.ToString() + " ");
                else
                {
                    string username = "Anonymous";
                    if (x.Message.ToLower().Contains("a potentially dangerous"))
                        System.Web.HttpContext.Current.Session["hacker"] = true;
                    if (System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] != null)
                        username = ((Employee)System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER]).Email;
                    LogError(x, "User = "+username + " URL = " + System.Web.HttpContext.Current.Request.RawUrl + " - Application Level Error : " + DateTime.Now.ToString() + " " + System.Web.HttpContext.Current.Request.UserHostAddress + " and " + ipaddress + " ");
                }

                if (System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] == null || System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID] == null)
                {
                    string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
                    if (!appPath.EndsWith("/"))
                        appPath += "/";
                    System.Web.HttpContext.Current.Response.Redirect(appPath + "account/logout");
                }
            }
            catch(Exception x) {
                LogError(x, "Error occured in Application_error handler --- " + x.Message);
            }

        }

        public void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Prevent Ajax requests from being returned the login form when not authenticated
            // (eg. after authentication timeout).
            if ((Request.Headers["X-Requested-With"] != null && Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                ||
                (Request["X-Requested-With"] != null && Request["X-Requested-With"] == "XMLHttpRequest"))
            {
                if (!Request.IsAuthenticated)
                {
                    Response.Clear();
                    Response.StatusCode = 403;
                    Response.Write("You Logged out");
                    Response.Flush();
                    Response.End();
                    return;
                }
            }
            if (User != null && User.Identity.IsAuthenticated)
            {
                string[] roles = Roles.GetRolesForUser("sanjuan\\" + User.Identity.Name.ToLower());
                HttpContext.Current.User = new GenericPrincipal(User.Identity, roles);
                if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME] == null)
                {

                    Session_Start(sender, e);
                }
            }

        }
        public void LogError(Exception x, string message = "")
        {
            string e = message + "***Exception Message : " + x.Message;
            if (x.InnerException != null)
            {
                e += "*** Inner Exception : " + x.InnerException.Message + " **Inner stack trace **" + x.InnerException.StackTrace;
            }
            e += "***Outer exception : *** " + x.StackTrace + "  *** SOURCE :" + x.Source;

            log.Error(e);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                OnlineUsers onlineusers = new OnlineUsers();
                if (Session != null && Session[CONSTANTS.SESSION_CURRENT_USER]!=null)
                {
                    string username = ((Employee)Session[CONSTANTS.SESSION_CURRENT_USER]).Email.ToLower().Replace("@avalara.com", "").Trim();
                    if (!onlineusers.UserOffline(username))
                    {
                        log.Error("Could not find user " + username + " in online users list");
                    }
                }
            }
            catch (Exception x)
            {
                log.Error("Error when removing user from online users list : " + x.Message + " *** " + x.StackTrace, x);
            }
        }
        protected void Session_Start(object sender, EventArgs e)
        {

            if (!Roles.RoleExists("Employee")) Roles.CreateRole("Employee");
            if (!Roles.RoleExists("Hr")) Roles.CreateRole("Hr");
            if (!Roles.RoleExists("HrAdmin")) Roles.CreateRole("HrAdmin");
            if (!Roles.RoleExists("Admin")) Roles.CreateRole("Admin");




        }
    }
}