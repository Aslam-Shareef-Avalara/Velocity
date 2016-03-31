using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcApplication2.Models
{
    public class BaseModel
    {
        public PEntities dbx = new PEntities();
        public System.Web.HttpContext ctx = System.Web.HttpContext.Current;
        public log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
