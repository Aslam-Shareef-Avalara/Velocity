using DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace MvcApplication2.Models
{
    public class Mail : BaseModel
    {
        public enum ACTION_TYPE
        {
            SET_YOUR_GOALS,
            SEND_GOALS_FOR_APPROVAL,
            APPROVE_GOALS,
            EVALUATE_SELF,
            SUBMIT_EVALUATION_TO_MANAGER,
            EVALUATE_REPORTEE,
            SEND_EVALUATION_TO_HR,
            APPROVE_EVALUATIONS,
            PUBLISH,
            GOALS_REJECTED,
            EVALUATION_REJECTED_BY_MANAGER,
            EVALUATION_REJECTED_BY_HR,
            GOALS_APPROVED,
            SUBMIT_FEEDBACK

        }

        private AlternateView GetReminderEmailBody(ACTION_TYPE actionType, Guid g, ref Employee ToEmp, ref string subject, long evalcycleid)
        {
            AlternateView alternateView = null;
            bool attachEmployeeImage = false;
            string mailbody = "<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <title>A Simple Responsive HTML Email</title> <style type='text/css'> body{{margin: 0; padding: 0; min-width: 100%!important;}}img{{height: auto;}}.content{{width: 100%; max-width: 600px;}}.header{{padding: 40px 30px 20px 30px;}}.innerpadding{{padding: 30px 30px 30px 30px;}}.borderbottom{{border-bottom: 1px solid #f2eeed;}}.subhead{{font-size: 15px; color: #ffffff; font-family: sans-serif; letter-spacing: 10px;}}.h1, .h2, .bodycopy{{color: #153643; font-family: sans-serif;}}.h1{{font-size: 33px; line-height: 38px; font-weight: bold;}}.h2{{padding: 0 0 15px 0; font-size: 24px; line-height: 28px; font-weight: bold;}}.bodycopy{{font-size: 16px; line-height: 22px;}}.button{{text-align: center; font-size: 18px; font-family: sans-serif; font-weight: bold; padding: 0 30px 0 30px;}}.button a{{color: #ffffff; text-decoration: none;}}.footer{{padding: 20px 30px 15px 30px;}}.footercopy{{font-family: sans-serif; font-size: 14px; color: #ffffff;}}.footercopy a{{color: #ffffff; text-decoration: underline;}}@media only screen and (max-width: 550px), screen and (max-device-width: 550px){{body[yahoo] .hide{{display: none!important;}}body[yahoo] .buttonwrapper{{background-color: transparent!important;}}body[yahoo] .button{{padding: 0px!important;}}body[yahoo] .button a{{background-color: #e05443; padding: 15px 15px 13px!important;}}body[yahoo] .unsubscribe{{display: block; margin-top: 20px; padding: 10px 50px; background: #2f3942; border-radius: 5px; text-decoration: none!important; font-weight: bold;}}}}/*@media only screen and (min-device-width: 601px){{.content{{width: 600px !important;}}.col425{{width: 425px!important;}}.col380{{width: 380px!important;}}}}*/ </style> </head> <body yahoo bgcolor='#f6f8f1'> <table width='100%' bgcolor='#f6f8f1' border='0' cellpadding='0' cellspacing='0'> <tr> <td><!--[if (gte mso 9)|(IE)]> <table width='600' align='center' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table bgcolor='#ffffff' class='content' align='center' cellpadding='0' cellspacing='0' border='0'> <tr> <td bgcolor='#c7d8a7' class='header'> <table width='70' align='left' border='0' cellpadding='0' cellspacing='0'> <tr> <td height='70' style='padding: 0 20px 20px 0;'> <img class='fix' src='cid:{5}' width='70' height='70' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> <table width='425' align='left' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table class='col425' align='left' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 425px;'> <tr> <td height='70'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='subhead' style='padding: 0 0 0 3px;'> AVALARA </td></tr><tr> <td class='h1' style='padding: 5px 0 0 0;'> VELOCITY </td></tr></table> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr><tr> <td class='innerpadding borderbottom'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='h2'> Hello &nbsp;{0}, </td></tr><tr> <td class='bodycopy' style='color:#bbbbbb;font-size:12px;' > This is an automated alert from VELOCITY on behalf of &nbsp;{1}. An action needs your immediate attention. </td></tr></table> </td></tr><tr> <td class='innerpadding borderbottom'> <table width='115' align='left' border='0' cellpadding='0' cellspacing='0'> <tr> <td height='115' style='padding: 0 20px 20px 0;'> <img class='fix' src='cid:{2}' width='115' height='115' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> <table width='380' align='left' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table class='col380' align='left' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 380px;'> <tr> <td> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='bodycopy' style='font-size:12px'>{3}</td></tr></table> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr><tr> <td class='innerpadding borderbottom' align='right'> <img align='right' class='fix' src='cid:{4}' width='120' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr></table> </body></html>";
            PEntities db = new PEntities();
            Employee TMG = new Employee() { FirstName = "TMG", LastName = "Bharat", Email = "tmg@avalara.com" };
            string evalcycleName = "";
            Stream ms = null, logoStream = null, mailiconStream = null;
            //Guid g = Guid.Parse("cf0d7fa6-a1df-4913-b869-a3abfbff8a02");
            var em = db.Employees.FirstOrDefault(x => x.gid == g);
            if (em.Active == false)
                return null;
            var mgr = db.Employees.FirstOrDefault(x => x.gid == em.Manager);
            LinkedResource employeeImage = null, logoImage = null, mailicon = null;
            logoStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/logo.png"), FileMode.Open, FileAccess.Read);
            logoImage = new LinkedResource(logoStream);

            mailiconStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/mailicon.gif"), FileMode.Open, FileAccess.Read);
            mailicon = new LinkedResource(mailiconStream);

            var eval = db.EvaluationCycles.Where(x => x.Id == evalcycleid).FirstOrDefault();
            if (eval != null)
            {
                evalcycleName = eval.Title;
            }
            Employee FromEmployee = null;
            logoImage.ContentId = Guid.NewGuid().ToString();
            mailicon.ContentId = Guid.NewGuid().ToString();
            string variableContent = "";

            switch (actionType)
            {
                case ACTION_TYPE.SET_YOUR_GOALS:
                    variableContent = string.Format("A new evaluation cycle ({0}) has started. Kindly go to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> and set your goals for this PE Cycle.", evalcycleName);
                    subject = "VELOCITY - Set Your Goals!";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.EVALUATE_SELF:
                    variableContent = string.Format("The evaluation cycle {0} has now entered the Evaluation phase. It is now time to evaluate yourself and get yourself reviewed by your manager. Kindly go to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> and fill in your self-evaluation against each goal that you had set in the beginning of this cycle.", evalcycleName);
                    subject = "VELOCITY - Time to evaluate yourself!";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SEND_GOALS_FOR_APPROVAL:
                    variableContent = string.Format("Gentle reminder to send your goals for approval to your manager for {0} . Please do not wait for until the last moment.", evalcycleName);
                    subject = "VELOCITY - REMINDER - Send your goals for approval.";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.APPROVE_GOALS:
                    variableContent = string.Format("Gentle reminder to approve goal of {0} for {1} . Please do not wait for until the last moment.", em.FirstName + " " + em.LastName, evalcycleName);
                    subject = "VELOCITY - REMINDER - Approve Goals for " + em.FirstName;
                    attachEmployeeImage = true;
                    ToEmp = mgr;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SUBMIT_EVALUATION_TO_MANAGER:
                    variableContent = string.Format("Gentle reminder to submit your self-evaluation for {0} to your manager . Please do not wait for until the last moment.", evalcycleName);
                    subject = "VELOCITY - REMINDER - Send self-evaluation for review";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SEND_EVALUATION_TO_HR:
                    variableContent = string.Format("Gentle reminder to send your review of {0}'s performance to TMG Bharat for Audit. Please do not wait for until the last moment.", em.FirstName + " " + em.LastName);
                    subject = "VELOCITY - REMINDER - Send " + em.FirstName + "'s review to HR";
                    attachEmployeeImage = true;
                    ToEmp = mgr;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SUBMIT_FEEDBACK:
                    variableContent = string.Format("Gentle reminder to submit your feedback for {0} via <a href='http://velocity.pn.avalara.net/'>VELOCITY</a>.", evalcycleName);
                    subject = "VELOCITY - REMINDER - Send Feedback";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                //case ACTION_TYPE.PUBLISH:
                //    variableContent = string.Format("Gentle reminder to publish HR approved evaluattions feedback {0} of {1} have been approved. Request you to log in to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> publish the evaluations/review after a meeting with the employee.", evalcycleName, em.FirstName + " " + em.LastName);
                //    subject = "VELOCITY - Evaluations Approved By HR";
                //    attachEmployeeImage = false;
                //    ToEmp = mgr;
                //    FromEmployee = TMG;
                //    break;
                default: return null;
            }



            if (attachEmployeeImage)
            {
                if (ToEmp == null)
                    FromEmployee = mgr;

                if (ToEmp != null)
                {
                    if (FromEmployee == null)
                        FromEmployee = ToEmp.gid.ToString() == mgr.gid.ToString() ? em : mgr;
                    if (em.ProfilePix != null && em.ProfilePix.Length > 0)
                    {
                        ms = new MemoryStream(em.ProfilePix);
                        employeeImage = new LinkedResource(ms);
                    }
                    else
                    {
                        ms = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/follower-man-user-login-round-512.png"), FileMode.Open, FileAccess.Read);
                        employeeImage = new LinkedResource(ms);
                    }
                    employeeImage.ContentId = Guid.NewGuid().ToString();
                }

            }
            else
            {
                ms = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/blank.png"), FileMode.Open, FileAccess.Read);
                employeeImage = new LinkedResource(ms);
                employeeImage.ContentId = Guid.NewGuid().ToString();
            }

            mailbody = string.Format(mailbody, ToEmp.FirstName, FromEmployee.FirstName + " " + FromEmployee.LastName, employeeImage.ContentId, variableContent, logoImage.ContentId, mailicon.ContentId);
            alternateView = AlternateView.CreateAlternateViewFromString(mailbody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(employeeImage);
            alternateView.LinkedResources.Add(logoImage);
            alternateView.LinkedResources.Add(mailicon);

            return alternateView;
        }
        private AlternateView GetMailBody(ACTION_TYPE actionType, Guid g, ref Employee ToEmp, ref string subject, long evalcycleid)
        {
            AlternateView alternateView = null;
            bool attachEmployeeImage = false;
            string mailbody = "<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/> <title>A Simple Responsive HTML Email</title> <style type='text/css'> body{{margin: 0; padding: 0; min-width: 100%!important;}}img{{height: auto;}}.content{{width: 100%; max-width: 600px;}}.header{{padding: 40px 30px 20px 30px;}}.innerpadding{{padding: 30px 30px 30px 30px;}}.borderbottom{{border-bottom: 1px solid #f2eeed;}}.subhead{{font-size: 15px; color: #ffffff; font-family: sans-serif; letter-spacing: 10px;}}.h1, .h2, .bodycopy{{color: #153643; font-family: sans-serif;}}.h1{{font-size: 33px; line-height: 38px; font-weight: bold;}}.h2{{padding: 0 0 15px 0; font-size: 24px; line-height: 28px; font-weight: bold;}}.bodycopy{{font-size: 16px; line-height: 22px;}}.button{{text-align: center; font-size: 18px; font-family: sans-serif; font-weight: bold; padding: 0 30px 0 30px;}}.button a{{color: #ffffff; text-decoration: none;}}.footer{{padding: 20px 30px 15px 30px;}}.footercopy{{font-family: sans-serif; font-size: 14px; color: #ffffff;}}.footercopy a{{color: #ffffff; text-decoration: underline;}}@media only screen and (max-width: 550px), screen and (max-device-width: 550px){{body[yahoo] .hide{{display: none!important;}}body[yahoo] .buttonwrapper{{background-color: transparent!important;}}body[yahoo] .button{{padding: 0px!important;}}body[yahoo] .button a{{background-color: #e05443; padding: 15px 15px 13px!important;}}body[yahoo] .unsubscribe{{display: block; margin-top: 20px; padding: 10px 50px; background: #2f3942; border-radius: 5px; text-decoration: none!important; font-weight: bold;}}}}/*@media only screen and (min-device-width: 601px){{.content{{width: 600px !important;}}.col425{{width: 425px!important;}}.col380{{width: 380px!important;}}}}*/ </style> </head> <body yahoo bgcolor='#f6f8f1'> <table width='100%' bgcolor='#f6f8f1' border='0' cellpadding='0' cellspacing='0'> <tr> <td><!--[if (gte mso 9)|(IE)]> <table width='600' align='center' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table bgcolor='#ffffff' class='content' align='center' cellpadding='0' cellspacing='0' border='0'> <tr> <td bgcolor='#c7d8a7' class='header'> <table width='70' align='left' border='0' cellpadding='0' cellspacing='0'> <tr> <td height='70' style='padding: 0 20px 20px 0;'> <img class='fix' src='cid:{5}' width='70' height='70' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> <table width='425' align='left' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table class='col425' align='left' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 425px;'> <tr> <td height='70'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='subhead' style='padding: 0 0 0 3px;'> AVALARA </td></tr><tr> <td class='h1' style='padding: 5px 0 0 0;'> VELOCITY </td></tr></table> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr><tr> <td class='innerpadding borderbottom'> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='h2'> Hello &nbsp;{0}, </td></tr><tr> <td class='bodycopy' style='color:#bbbbbb;font-size:12px;' > This is an automated alert from VELOCITY on behalf of &nbsp;{1}. An action needs your immediate attention. </td></tr></table> </td></tr><tr> <td class='innerpadding borderbottom'> <table width='115' align='left' border='0' cellpadding='0' cellspacing='0'> <tr> <td height='115' style='padding: 0 20px 20px 0;'> <img class='fix' src='cid:{2}' width='115' height='115' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> <table width='380' align='left' cellpadding='0' cellspacing='0' border='0'> <tr> <td><![endif]--> <table class='col380' align='left' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 380px;'> <tr> <td> <table width='100%' border='0' cellspacing='0' cellpadding='0'> <tr> <td class='bodycopy' style='font-size:12px'>{3}</td></tr></table> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr><tr> <td class='innerpadding borderbottom' align='right'> <img align='right' class='fix' src='cid:{4}' width='120' border='0' alt=''/> </td></tr></table><!--[if (gte mso 9)|(IE)]> </td></tr></table><![endif]--> </td></tr></table> </body></html>";
            PEntities db = new PEntities();
            Employee TMG = new Employee() { FirstName = "TMG", LastName = "Bharat", Email = "tmg@avalara.com" };
            string evalcycleName = "";
            Stream ms = null, logoStream = null, mailiconStream = null;
            //Guid g = Guid.Parse("cf0d7fa6-a1df-4913-b869-a3abfbff8a02");
            var em = db.Employees.FirstOrDefault(x => x.gid == g);
            if (em.Active == false)
                return null;
            var mgr = db.Employees.FirstOrDefault(x => x.gid == em.Manager);
            LinkedResource employeeImage = null, logoImage = null, mailicon = null;
            logoStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/logo.png"), FileMode.Open, FileAccess.Read);
            logoImage = new LinkedResource(logoStream);

            mailiconStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/mailicon.gif"), FileMode.Open, FileAccess.Read);
            mailicon = new LinkedResource(mailiconStream);

            var eval = db.EvaluationCycles.Where(x => x.Id == evalcycleid).FirstOrDefault();
            if (eval != null)
            {
                evalcycleName = eval.Title;
            }
            Employee FromEmployee = null;
            logoImage.ContentId = Guid.NewGuid().ToString();
            mailicon.ContentId = Guid.NewGuid().ToString();
            string variableContent = "";
            switch (actionType)
            {
                case ACTION_TYPE.APPROVE_GOALS:
                    variableContent = string.Format("I have submitted the &nbsp;{0} goals for your approval. Request you to log in to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> and approve the goals at your convenience.", evalcycleName);
                    subject = "VELOCITY - Goals Submitted";
                    attachEmployeeImage = true;
                    ToEmp = mgr;
                    FromEmployee = em;
                    break;
                case ACTION_TYPE.APPROVE_EVALUATIONS: variableContent = string.Format("I have evaluated and submitted my review of &nbsp;{0}'s performance. Please do an audit of the ratings and evaluations.", em.FirstName + " " + em.LastName);
                    subject = "VELOCITY - Evaluation Pending Approval";
                    attachEmployeeImage = true;
                    ToEmp = new Employee() { FirstName = "TMG", LastName = "Bharat", Email = "tmg@avalara.com" };
                    FromEmployee = mgr;
                    break;
                case ACTION_TYPE.EVALUATE_REPORTEE:
                    variableContent = string.Format("I have submitted the &nbsp;{0} self-evaluations to you. Request you to log in to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> review my performance at your convenience.", evalcycleName);
                    subject = "VELOCITY - Self-Evaluation Submitted";
                    attachEmployeeImage = true;
                    ToEmp = mgr;
                    FromEmployee = em;
                    break;
                case ACTION_TYPE.EVALUATE_SELF:
                    variableContent = string.Format("Please begin your self evaluation at the earliest for {0}.", evalcycleName);
                    subject = "VELOCITY - Evaluation Phase Started";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.PUBLISH:
                    variableContent = string.Format("Evaluation for {0} of {1} have been approved. Request you to log in to <a href='http://velocity.pn.avalara.net/'>VELOCITY</a> publish the evaluations/review after a meeting with the employee.", evalcycleName, em.FirstName + " " + em.LastName);
                    subject = "VELOCITY - Evaluations Approved By HR";
                    attachEmployeeImage = false;
                    ToEmp = mgr;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SEND_EVALUATION_TO_HR:
                    variableContent = string.Format("Please send your review for {0} of {1} to HR for audit.", evalcycleName, em.FirstName + " " + em.LastName);
                    subject = "VELOCITY - Send Evaluations To HR";
                    attachEmployeeImage = false;
                    ToEmp = mgr;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SEND_GOALS_FOR_APPROVAL:
                    variableContent = string.Format("Please send your goal for {0} to your manager.", evalcycleName);
                    subject = "VELOCITY - Send Goals For Approval";
                    attachEmployeeImage = false;
                    ToEmp = mgr;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SET_YOUR_GOALS:
                    variableContent = string.Format("Please freeze your goals for {0} on or before {1}", evalcycleName, eval.GoalEndDate.Value.ToShortDateString());
                    subject = "VELOCITY - Set Your Goals";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SUBMIT_EVALUATION_TO_MANAGER:
                    variableContent = string.Format("Please submit your evaluations for {0} to your manager at the earliest", evalcycleName);
                    subject = "VELOCITY - Submit Your Evaluations to Manager";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.GOALS_REJECTED:
                    variableContent = "The goals submitted by you have been rejected. Please review and re-submit the same.";
                    subject = "VELOCITY - Your Goals Have Been Rejected!";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.EVALUATION_REJECTED_BY_MANAGER:
                    variableContent = "The self-evaluation submitted by you have been rejected. Please review and re-submit the same.";
                    subject = "VELOCITY - Your Evaluations Have Been Rejected!";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.EVALUATION_REJECTED_BY_HR:
                    variableContent = string.Format("The evaluation submitted by you for {0} has been rejected during TMG Audit. Please review and re-submit the same.", em.FirstName + " " + em.LastName);
                    subject = "VELOCITY - Evaluations Rejected By HR!";
                    attachEmployeeImage = false;
                    ToEmp = mgr;
                    break;
                case ACTION_TYPE.GOALS_APPROVED:
                    variableContent = "The goals submitted by you has been approved by your Manager. Now you can login to Velocity to check the approved goals.";
                    subject = "VELOCITY - Goals Approved!";
                    attachEmployeeImage = true;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;
                case ACTION_TYPE.SUBMIT_FEEDBACK:
                    variableContent = "The process feedback is pending from your end.  To complete the PE cycle, please submit the same at the earliest.";
                    subject = "VELOCITY - Feedback required.";
                    attachEmployeeImage = false;
                    ToEmp = em;
                    FromEmployee = TMG;
                    break;

            }


            if (attachEmployeeImage)
            {
                if (ToEmp == null)
                    FromEmployee = mgr;

                if (ToEmp != null)
                {
                    if (FromEmployee == null)
                        FromEmployee = ToEmp.gid.ToString() == mgr.gid.ToString() ? em : mgr;
                    if (FromEmployee.ProfilePix != null && FromEmployee.ProfilePix.Length > 0)
                    {
                        ms = new MemoryStream(FromEmployee.ProfilePix);
                        employeeImage = new LinkedResource(ms);
                    }
                    else
                    {
                        ms = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/follower-man-user-login-round-512.png"), FileMode.Open, FileAccess.Read);
                        employeeImage = new LinkedResource(ms);
                    }
                    employeeImage.ContentId = Guid.NewGuid().ToString();
                }

            }
            else
            {
                ms = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/images/blank.png"), FileMode.Open, FileAccess.Read);
                employeeImage = new LinkedResource(ms);
                employeeImage.ContentId = Guid.NewGuid().ToString();
            }

            mailbody = string.Format(mailbody, ToEmp.FirstName, FromEmployee.FirstName + " " + FromEmployee.LastName, employeeImage.ContentId, variableContent, logoImage.ContentId, mailicon.ContentId);
            alternateView = AlternateView.CreateAlternateViewFromString(mailbody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(employeeImage);
            alternateView.LinkedResources.Add(logoImage);
            alternateView.LinkedResources.Add(mailicon);

            return alternateView;

        }
        public bool SendEmail(ACTION_TYPE actionType, Guid employeeId, long evalcycleid = 0, bool isReminder = false)
        {
            string notificationStatus = (string)System.Web.HttpContext.Current.Session[CONSTANTS.NOTIFICATION_STATUS];
            if (string.IsNullOrEmpty(notificationStatus) || notificationStatus.ToLower() == "off")
                return true;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("10.240.30.5");
                Employee employee = new Employee();
                string subject = "";
                AlternateView alternateView = null;
                if (!isReminder)
                    alternateView = GetMailBody(actionType, employeeId, ref employee, ref subject, evalcycleid);
                else
                    alternateView = GetReminderEmailBody(actionType, employeeId, ref employee, ref subject, evalcycleid);
                if (alternateView == null)
                    return false;
                mail.From = new MailAddress("donotreply@avalara.com", "Velocity");
                if (employee.Email.ToLower().Trim() == "sudhir.singh@avalara.com" || employee.Email.ToLower().Trim() == "peter.horadan@avalara.com")
                    return true;
                mail.To.Add(employee.Email.ToLower().Trim());
                mail.Subject = subject;
                mail.AlternateViews.Add(alternateView);
                mail.IsBodyHtml = true;
                SmtpServer.Port = 25;
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error sending email : " + ex.Message + " --> " + ex.StackTrace, ex);
                return false;
            }
        }
        public bool SendEmail(string mailTitle, string mailBody)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("10.240.30.5");
                Employee employee = new Employee();
                string subject = "";
                AlternateView alternateView = null;
                mail.From = new MailAddress("donotreply@avalara.com", "Velocity");
                
                mail.To.Add("aslam.shareef@avalara.com");
                mail.Subject = mailTitle;
                mail.Body = mailBody;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 25;
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error sending email : " + ex.Message + " --> " + ex.StackTrace, ex);
                return false;
            }
        }
    }
}