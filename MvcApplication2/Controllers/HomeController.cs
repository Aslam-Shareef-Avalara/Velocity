using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication2.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(int? id)
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
           
            // A authentication header must be supplied. This header can be changed to Negotiate when using keberos authentication
            

          
            
            return Redirect("~/employee/aboutme");

        }
        public ActionResult offline()
        {
            
            return View("offline");
        }

        [HttpPost]
        public JsonResult KeepAlive()
        {
            if (!string.IsNullOrEmpty(ViewBag.ShutdownMessage) && (ctx.Session["warningshown"]==null || (bool)ctx.Session["warningshown"] == false))
            {
                ctx.Session["warningshown"] = true;
                return Json(new {scheduled=true},JsonRequestBehavior.AllowGet);
            }
            return Json(new {isoffline=IsOffline},JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScheduledDowntimeWarning()
        {
            return PartialView("MaintenenceWarning");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
      
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
        public void addtosession(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            ctx.Session[key] = value;
        }

        public string getSessionValue(string key)
        {
            if (string.IsNullOrEmpty(key) || ctx.Session[key]==null)
                return "";

            return ctx.Session[key].ToString();
        }

        public void markasexpert()
        {
            IEmployeeService employeeService = new EmployeeService(OrgId, AppName,currentUser);
            Employee emp =employeeService.MarkAsExpertUser(currentUser.gid);
            if (emp != null)
            {
                currentUser = emp;
            }
            
        }
    }
}
