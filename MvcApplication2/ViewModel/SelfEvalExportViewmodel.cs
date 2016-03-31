using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class SelfEvalExportViewmodel
    {
        public List<EmployeeEvaluation> SelfEvals = new List<EmployeeEvaluation>();
        public List<ManagerEvaluation> MgrEvals = new List<ManagerEvaluation>();
        public List<Goal> Goals = new List<Goal>();
        public EvaluationCycle Evalcycle = new EvaluationCycle();
        public string OrgName = "";
        public string ManagerName = "";
        public Employee Employee = new Employee();

    }
}