using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using sec=System.Web.Security;

namespace MvcApplication2.Controllers
{
    public class UsersController : BaseController
    {
        //
        // GET: /Users/
        public ActionResult Roles()
        {
            return View(sec.Roles.GetAllRoles());
        }

        public ActionResult AddRole(string roleName)
        {
            if (!sec.Roles.RoleExists(roleName))
            {
                sec.Roles.CreateRole(roleName);
                TempData["messsage"] = roleName + " role created successfully";
            }
            else {
                TempData["messsage"] = roleName + " role already exists!";
            }
            return RedirectToAction("Roles");
        }

        public ActionResult AllUsersInRole(string role)
        {
            return View(sec.Roles.GetUsersInRole(role));
        }

        public JsonResult GetAllUsers()
        {
            return Json(db.aspnet_Users.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddUserToRole(string username, string role)
        {
            if (!sec.Roles.IsUserInRole(username, role))
                sec.Roles.AddUserToRole(username, role);
            else
                return Json((object)username + " is already in " + role + " role");

            return Json((object)username + " added to " + role + " role");
        }

        public JsonResult RemoveUserFromRole(string username, string role)
        {
            if (sec.Roles.IsUserInRole(username, role))
                sec.Roles.RemoveUserFromRole(username, role);
            else
                return Json((object)username + " is not in " + role + " role");

            return Json((object)username + " removed from " + role + " role");
        }

        public JsonResult GetRolesForUser(string username)
        {
            return Json(sec.Roles.GetRolesForUser(username), JsonRequestBehavior.AllowGet);
        }

	}
}