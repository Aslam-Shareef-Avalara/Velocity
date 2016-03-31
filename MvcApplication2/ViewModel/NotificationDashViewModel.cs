using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class NotificationDashViewModel
    {
        public string ReminderType;
        public string ReminderTypeDisplay { get { return _reminderType; } set {
            switch (value)
            {
                case "SET_YOUR_GOALS": _reminderType = "Set Goals";
                    break;
                case "EVALUATE_SELF": _reminderType = "Begin Evaluation";
                    break;
                case "SEND_GOALS_FOR_APPROVAL": _reminderType = "Submit Goals";
                    break;
                case "APPROVE_GOALS": _reminderType = "Approve Goals";
                    break;
                case "SUBMIT_EVALUATION_TO_MANAGER": _reminderType = "Submit Self-Evaluation";
                    break;
                case "SEND_EVALUATION_TO_HR": _reminderType = "Submit To HR";
                    break;
                case "SUBMIT_FEEDBACK": _reminderType = "Send Process Feedback";
                    break;
            }
        } }
        private string _reminderType = "";
        public DateTime? LastRun = new DateTime();
        public int Since = 0;
        public long NotificationCount = 0;
    }
}