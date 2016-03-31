using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class EmployeeViewModel:EmployeeExtended
    {
        public EmployeeViewModel(Employee e):base(e)
        {

        }
        private EmployeeService empService = new EmployeeService(Convert.ToInt32(System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_ORG_ID]),System.Web.HttpContext.Current.Session[CONSTANTS.APPLICATION_NAME].ToString());
        public string ManagerName {
            get {
                return empService.GetEmployee(Manager.Value).FirstName + " " + empService.GetEmployee(Manager.Value).LastName;
            }
            
        }
        public List<Employee> GetAllEmployees()
        {
            var allemps =  empService.GetAllEmployees();
            if (allemps == null)
            {
                return new List<Employee>();
            }
            else
                return allemps.ToList();
        }
    }
}