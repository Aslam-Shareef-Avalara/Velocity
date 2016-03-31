using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public static class EvaluationType
    {
        public const string SELF_EVALUATION = "Self Evaluation";
        public const string MANAGER_EVALUATION = "Manager Evaluation";
        public const string HR_EVALUATION = "Hr Evaluation";
    }

    public static class BadgeType
    {
        public const string BEGIN_GOAL_SETTING = "Begin Goal Setting";
        public const string BEGIN_SELF_EVALUATION = "Begin Self Evaluation";
        public const string PUBLISH_GOALS = "Publish Goals";
        public const string PUBLISH_SELF_EVAL = "Publish Self Evaluation";
        public const string GOALS_PUBLISHED="Approve Goals";
        public const string GOALS_APPROVED ="Goals Approved";
        public const string EVALUATION_SUBMITTED = "Self Evaluation Complete";
        public const string EMPLOYEE_EVALUATED = "Evaluated Employee";
        public const string EVALUATION_APPROVED = "Evaluation Approved";
        public const string EVALUATION_REJECTED = "Evaluation Rejected";
        public const string GOALS_REJECTED = "Goals Rejected";
        public const string GIVE_FEEDBACK = "Give Feedback";
    }
    public enum GoalCycleStatus {
        NO_ACTION_REQUIRED=1000,
        NOT_STARTED=0,
        STARTED=25,
        PUBLISHED=50,
        MANAGER_STARTED=75,
        GOALS_SET=100   
    }
    public enum EvalCycleStatus {
        NO_ACTION_REQUIRED=1000,
        NOT_STARTED=0,
        STARTED = 20,
        PUBLISHED = 40,
        MANAGER_STARTED = 60,
        MANAGER_APPROVED = 80,
        HR_APPROVED = 90,
        COMPLETED = 100
    }
    public enum PECycle{
        GOAL_SETTING,
        EVALUATION,
        NONE
    }
    public interface IEmployeeService
    {
        void CreateEmployee(Employee employee,string username);
        Employee GetEmployee(Guid empId);
        Employee GetEmployeeUsingUsername(string username);
        List<EmployeeExtended> GetReportees(Guid managerId);
        Employee GetManager(Guid empId);
        void SetReportingStructure(Guid guid, Hashtable reportees);
        Hashtable GetPECycleStatus(Guid empId);
        List<Employee> OrgStructure();
        List<Employee> GetReporteesOf(Guid empid);
        bool IsFirstTimeUser(Guid employeeid);
        Employee  MarkAsExpertUser(Guid employeeid);

        List<Employee> GetCoworkers(Guid empId);

        List<EmployeeExtended> GetReviewies(Guid reviewerId);
        IQueryable<Employee> GetAllEmployees();
        //Hashtable GetPECycleStatus();
    }
}


