using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcApplication2.Controllers
{
    public class EmployeeEvaluationStatusModel
    {
        public Guid EmployeeId;
        public string Name;
        public int Status;
        public Guid ManagerId;
        public string ManagerName;
        public string EmpOrgId;
    }

    public class EmployeeEvaluationListModel
    {
        public List<EmployeeEvaluationStatusModel> EmployeeEvaluationStatus;
        public PECycle PEType;
        public long PECycleId;
        public string PECycleTitle;
    }
}
