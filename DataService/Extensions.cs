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
        public static string GetPhase(this EvaluationCycle cycle)
        {
            if (cycle != null)
            {
                if (cycle.GoalStartDate <= DateTime.Today && cycle.GoalEndDate >= DateTime.Today)
                {
                    return "Goal Setting Phase";
                }
                else if (cycle.EvaluationStart <= DateTime.Today && cycle.EvaluationEnd >= DateTime.Today)
                    return "Evaluation Phase";
                else return "";
            }
            else return "";
        }
    }
}
