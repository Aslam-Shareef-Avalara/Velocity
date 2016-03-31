using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class GoalsViewModel
    {
        public EvaluationCycle PECycle = new EvaluationCycle();
        public List<Goal> Goals = new List<Goal>();
        public EvalCycleStatus EvaluationStatus = EvalCycleStatus.NO_ACTION_REQUIRED;
        public GoalCycleStatus GoalSettingStatus = GoalCycleStatus.NO_ACTION_REQUIRED;
        public List<ManagerEvaluation> ManagerEvaluations = new List<ManagerEvaluation>();
        public Guid EmployeeId = new Guid();
        public bool Editable = false;
        public bool AllowAdd = false;
        public PECycle PE_Cycle;
        public string ButtonToShow = "None";
        public int TotalWeightage = 0;
        public string CurrentStatus = "";
    }
}