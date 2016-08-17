using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MvcApplication2.Models
{
    public static class ExtensionsAndHelpers
    {

        public static string LegitimizeString(this string str)
        {
            Regex reg = new Regex("((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))");
            return reg.Replace(str, " $1");
        }
    }
}

