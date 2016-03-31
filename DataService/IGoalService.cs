using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public interface IGoalService
    {
        void PublishGoalsToManager(Guid? employeeId, long evalCycleId = 0);
        void SaveGoals(Guid? employeeId, long evalCycleId = 0);
        void SaveGoalAsDraft(Goal g);
        void ApproveGoals(Guid? employeeId, Guid ManagerId, long evalCycleId = 0);
        List<Goal> GetGoals(Guid employeeId, long evalCycleId = 0);

        List<Goal> GetSelfGoals(Guid employeeId, long evalCycleId = 0);
        long GetGoalSettingCycle();
        List<Goal> GetFixedGoals();

        
    }
}
