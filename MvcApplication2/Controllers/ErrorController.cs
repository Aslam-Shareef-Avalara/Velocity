using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication2.Controllers
{
    [AllowAnonymous]
    public class ErrorController :  BaseController
    {
        //
        // GET: /Error/
        [AllowAnonymous]
        public ActionResult whattheheck()
        {
            return View("FileNotFound");
        }

        public ActionResult YouAreSoScrewed()
        {
            return View("AccessDenied");
        }

        public ActionResult WeBrokeSomething()
        {
            return View("InternalServerError");
        }
	}
}