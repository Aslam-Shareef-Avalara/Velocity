using DataService;
using HtmlAgilityPack;
using MvcApplication2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication2.Controllers
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class AccountController : BaseController
    {
        private bool Authenticate(string userName, string password, string domain, out DirectoryEntry de)
        {
            bool authentic = false;
            de = null;
            try
            {
                OnlineUsers onlineusers = new OnlineUsers();
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain,
                    userName, password);
                object nativeObject = entry.NativeObject;
                de = entry;
                string uname = userName.ToLower().Replace("sanjuan\\", "").Trim();
                //ActiveDirectoryHelper adh = new ActiveDirectoryHelper();
                onlineusers.NewOnlineUser=uname;
                //adh.GetUserDetails(entry);

                authentic = true;
            }
            catch (DirectoryServicesCOMException x)
            {
                LogError(x, "Login failed for " + userName);
            }

            return authentic;
        }
        //
        // GET: /Account/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult login()
        {
            string festivalname = "";
            string loginView = "login2";
            festivalname = ctx.Request.QueryString["f"];

            if (!string.IsNullOrWhiteSpace(festivalname))
            {
                switch (festivalname.ToLower())
                {
                    case "eid": loginView = "eidlogin";
                        break;
                    case "diwali": loginView = "diwalilogin";
                        break;
                    case "christmas": loginView = "christmaslogin";
                        break;
                    case "rakhi": loginView = "rakhilogin";
                        break;
                    case "holi": loginView = "holilogin";
                        break;
                    case "rday": loginView = "republicdaylogin";
                        break;
                    case "iday": loginView = "independencedaylogin";
                        break;
                    case "newyear": loginView = "newyearlogin";
                        break;
                }
            
            }
           
            loginView = string.IsNullOrWhiteSpace(festivalname) ? GetLoginView() : loginView;



            return View(loginView);
        }
       

       
       
        

        

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = "~/")
        {
            DirectoryEntry de = null;
            string festivalname = "";
            festivalname = ctx.Request.QueryString["f"];
            festivalname = string.IsNullOrWhiteSpace(festivalname) ? "login2" : festivalname;
            if (festivalname != "login2")
            {
                switch (festivalname.ToLower())
                {
                    case "eid": festivalname = "eidlogin";
                        break;
                    case "diwali": festivalname = "diwalilogin";
                        break;
                    case "christmas": festivalname = "christmaslogin";
                        break;
                    case "rakhi": festivalname = "rakhilogin";
                        break;
                }

            }//OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com
            if (model != null && Authenticate(model.UserName, model.Password, "DC=SanJuan,DC=Avalara,DC=com", out de))
            {



                string un = model.UserName.ToLower();//.Replace("sanjuan\\", "");
                if (!un.Contains("sanjuan"))
                {
                    un = "sanjuan\\" + un;
                }
                if (un.Contains("@"))
                {
                    ModelState.AddModelError("", "The user name cannot be email address");
                    return View(GetLoginView(), model);
                }

                FormsAuthentication.SetAuthCookie(un, true);
                ctx.Session["adobj"] = de;
                return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, false));
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(GetLoginView(), model);
        }

        private string GetLoginView()
        {
            string loginView = "login2";
            
            if (ctx.Cache["showholidays"] == null)
                ctx.Cache["showholidays"] = ConfigurationManager.AppSettings["show holidays"] ?? "on";
            if ((string)ctx.Cache["showholidays"] == "on")
            {
                Festival festival = new Festival(ctx);
                festival.GetEventDate< FestivalSourceOfficeHoldaysDotCom>(new string[] { "Fitar", "Fitr" });
                festival.GetEventDate<FestivalSourceOfficeHoldaysDotCom>(new string[] { "Diwali", "Deepavali" });
                festival.GetEventDate<FestivalSourceOfficeHoldaysDotCom>(new string[] { "Adha", "Azha", "Bakri", "Juha" });
                festival.GetEventDate<FestivalSourceOfficeHoldaysDotCom>(new string[] { "Rakhi", "Raksha" });
                festival.GetEventDate<FestivalSourceOfficeHoldaysDotCom>(new string[] { "Holi" });
                loginView = festival.GetFestival().LoginView;
            }
            return loginView;
        }
        public ActionResult logout()
        {
            OnlineUsers onlineusers = new OnlineUsers();
            if (currentUser != null)
            {
                onlineusers.UserOffline(currentUser.Email.ToLower().Replace("@avalara.com", "").Trim());
            }
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            //FormsAuthentication.RedirectToLoginPage();
            return Redirect("~/");
        }
    }
}