using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public interface IEmployeeEvaluationService
    {
        void DraftSelfEvaluation(string comment, Guid goalid, int ratingId, Guid empid, long evalCycleId);
        void SaveSelfEvaluation(Guid employeeId, long evalCycleId = 0);
        void PublishSelfEvaluation(Guid employeeId, long evalCycleId = 0);
        List<EmployeeEvaluation> GetSelfEvaluations(Guid employeeId, long evalCycleId = 0);
        List<ManagerEvaluation> GetManagersEvaluation(Guid employeeId, long evalCycleId = 0);
        EvaluationRating GetOverallEvaluationRating(Guid employeeId, long evalCycleId = 0);

        Guid SaveAttachment(Guid selfEvalId, byte[] fileContents, string fileName, string fileType);
        List<Attachment> GetAttachments(Guid selfEvalId);
        bool DeleteAttachment(Guid attachmentId);
        List<EvaluationCycle> GetAllCycles(Guid? empid=null);
        bool IsEvaluationComplete(Guid employeeId, long evalCycleId = 0);
        
    }

    public interface IManagerEvaluationService
    {
        void DraftReview(Guid goalId, Guid managerId, string comment, int rating, Guid employeeId, long evalcycleid);
        void SaveReview(Guid employeeId, long evalcycleid);
        void PublishReview(Guid employeeId, long evalcycleid);
        List<ManagerEvaluation> GetReviews(Guid employeeId, long evalCycleId = 0);
        List<EvaluationRating> GetReviewStatus(Guid? employeeId=null, long evalCycleId = 0);
        void SaveFinalReviewComment(string ReviewComment, Guid employeeId, long evalCycleId = 0);
        EvaluationRating CalculateAvgRating(Guid? employeeId = null, long evalCycleId = 0, Guid? goalid=null);
        bool RejectEvaluations(int resetStep,Guid? employeeId = null, long evalCycleId = 0);

        bool Publish(Guid employeeId, long evalcycleid);

    }

    public interface IHrEvaluationService
    {
        List<Employee> GetEmployeesWhoHaveNotStartedEvalCycle(long evalCycleId = 0);
        List<Employee> GetEmployeesWhoHaveNotPublishedComments(long evalCycleId = 0);
        List<Employee> GetManagersWhoHaveNotStartedReviews(long evalCycleId = 0);
        List<Employee> GetManagersWhoHaveNotPublishedReviews(long evalCycleId = 0);
        EvaluationCycle StartPECycle(EvaluationCycle evalCycle);

        EvaluationCycle EditPECycle(EvaluationCycle evalCycle); 

        List<EvaluationCycle> GetActivePECycles();

        void PublishEvaluation(Guid empid, long evalCycleId);

        List<EvaluationCycle> GetValidPECycles();
        void RejectManagersEvaluation(Guid employeeId, long evalcycleId, string reason);

    }
}
