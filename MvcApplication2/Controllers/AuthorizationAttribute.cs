using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication2.Controllers
{
    public class AuthLogAttribute : AuthorizeAttribute
    {
        public AuthLogAttribute()
        {
            View = "AuthorizeFailed";
        }

        public string View { get; set; }
        public string RedirectAction { get; set; }
        public string RedirectController { get; set; }

        private bool Authenticate(AuthorizationContext ActionContext)
        {
            IPrincipal principal;
            if (TryGetPrincipal(ActionContext.RequestContext.HttpContext.User.Identity.Name,out principal ))
            {
                HttpContext.Current.User = principal;
                return true;
            }
            return false;
        }




        private bool TryGetPrincipal(string Username, out IPrincipal Principal)
        {
            Username = Username.Trim();
                //check if in allowed role
                bool isAllowedRole = false;
                string[] userRoles = System.Web.Security.Roles.GetRolesForUser(Username);
                string[] allowedRoles = Roles.Split(',');  //Roles is the inherited AuthorizeAttribute.Roles member
                foreach (string userRole in userRoles)
                {
                    foreach (string allowedRole in allowedRoles)
                    {
                        if (userRole.Trim() == allowedRole.Trim())
                        {
                            isAllowedRole = true;
                        }
                    }
                }

                if (!isAllowedRole)
                {
                    Principal = null;
                    return false;
                }



                Thread.CurrentPrincipal = Principal = new GenericPrincipal(new GenericIdentity(Username), userRoles); 

                return true;
            }
            
        

        /// <summary>
        /// Check for Authorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Authenticate(filterContext))
                return;
            
            //base.OnAuthorization(filterContext);
            IsUserAuthorized(filterContext);
        }

        /// <summary>
        /// Method to check if the user is Authorized or not
        /// if yes continue to perform the action else redirect to error page
        /// </summary>
        /// <param name="filterContext"></param>
        private void IsUserAuthorized(AuthorizationContext filterContext)
        {
            // If the Result returns null then the user is Authorized 
          

            //If the user is Un-Authorized then Navigate to Auth Failed View 
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(RedirectAction) && !string.IsNullOrEmpty(RedirectController))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary {
                                { "controller", RedirectController },
                                { "action", RedirectAction }});
                    return;
                }
                // var result = new ViewResult { ViewName = View };
                var vr = new ViewResult();
                vr.ViewName = View;

                ViewDataDictionary dict = new ViewDataDictionary();
                dict.Add("Message", "Sorry you are not authorized to perform this action");

                vr.ViewData = dict;

                var result = vr;

                filterContext.Result = result;
            }
        }
    }
}