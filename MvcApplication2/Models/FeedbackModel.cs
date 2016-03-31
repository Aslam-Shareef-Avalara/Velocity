using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class FeedbackModel:BaseModel
    {
        public List<FeedbackQuestion> Questions = new List<FeedbackQuestion>();
        public List<FeedbackAnswer> Answers = null;
        public List<FeedbackAnswerOption> Options = new List<FeedbackAnswerOption>();
        public EvaluationCycle Evalcycle = new EvaluationCycle();
        public bool IsEditable = true;
    }
}