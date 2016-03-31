using DataService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class AboutMe
    {
        public Employee Myself;
        public string Designation;
        public Organization Org ;
        public Employee HrPerson;
        public Employee Manager;
        public List<Employee> Coworkers;
        public Hashtable Reportees;
    }
}