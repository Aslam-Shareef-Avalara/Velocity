using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using DataService;

namespace DataWorkFlow
{

    public sealed class EmployeeGoalSetting : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> ActionCommand { get; set; }
        public InArgument<Guid> EmpId { get; set; }
        public InArgument<long> PEId { get; set; }

        //IGoalService goalService = new GoalService();

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            
        }
    }
}
