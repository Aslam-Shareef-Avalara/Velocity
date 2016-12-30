using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public static class Extensions
    {
        public static string FullName(this Employee emp)
        {
            if (emp != null)
            {
                return emp.FirstName + " " + emp.LastName;
            }
            else return "";
        }
    }
}
