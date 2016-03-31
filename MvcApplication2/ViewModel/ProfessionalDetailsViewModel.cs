using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class ProfessionalDetailsViewModel
    {
        public List<EmploymentHistory> Employment = new List<EmploymentHistory>();
        public List<EmployeeEducation> Education = new List<EmployeeEducation>();
        public List<EmployeeSkill> Skills = new List<EmployeeSkill>();
        Employee Employee = new Employee();
    }
}