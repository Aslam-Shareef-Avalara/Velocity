using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class EvaluationViewModel
    {
        public List<Goal> Goals = new List<Goal>();
        public List<EmployeeEvaluation> SelfEvaluations = new List<EmployeeEvaluation>();
        public List<ManagerEvaluation> ManagerEvaluations = new List<ManagerEvaluation>();
        public List<Attachment> Attachments = new List<Attachment>();
        public EvaluationRating Rating = new EvaluationRating();
        public string EvalType = EvaluationType.SELF_EVALUATION;
        public Employee EmployeeDetails = new Employee();
        public Employee Manager = new Employee();
        public bool IsEditable = true;
        public EvaluationConclusion Conclusion = new EvaluationConclusion();
        public List<FeedbackAnswer> Feedbackanswers = new List<FeedbackAnswer>();
        public List<FeedbackQuestion> FeedbackQuestions = new List<FeedbackQuestion>();
        public List<FeedbackAnswerOption> Options = new List<FeedbackAnswerOption>();
        public int Status
        {
            get
            {
                if (Goals != null && Goals.Any(x => !x.Fixed))
                {
                    return Goals.First().Status.Value;
                }
                else
                    return GoalStatus.NOT_STARTED;
            }
        }

        
    }
}