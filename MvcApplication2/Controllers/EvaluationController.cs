using DataService;
using MvcApplication2.Models;
using MvcApplication2.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication2.Controllers
{
    public class EvaluationController : BaseController
    {
        IEmployeeEvaluationService empEvalService = null;
        IGoalService goalservice = null;
        EvaluationViewModel evaluationViewModel = new EvaluationViewModel();
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            empEvalService = new EmployeeEvaluationService(OrgId, AppName,currentUser);
            goalservice = new GoalService(OrgId, AppName,currentUser);
            base.Initialize(requestContext);
        }

        public EvaluationController()
        {

        }

        public JsonResult reject(string reporteeId, int resetStep)
        {
            Guid reportee_id = Guid.Parse(reporteeId);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            try
            {
                var rejectedMessages = db.RejectedMessages.Where(x => x.ManagerID == currentUser.gid &&
                 x.EvalCycleId == currentEvalCycleId && x.EvaluationPhase == PECycle.EVALUATION.ToString()
                 && x.EmployeeId == reportee_id
                 ).ToList();
                if (rejectedMessages != null && rejectedMessages.Count > 0)
                {
                    db.RejectedMessages.RemoveRange(rejectedMessages);
                    db.SaveChanges();
                }
            }
            catch (Exception x)
            {
                LogError(x, "While rejecting for " + reporteeId);
            }
            try
            {

                new ManagerEvaluationService(OrgId, AppName, currentUser).RejectEvaluations(resetStep, reportee_id, currentEvalCycleId);
                new Mail().SendEmail(Mail.ACTION_TYPE.EVALUATION_REJECTED_BY_MANAGER, reportee_id, currentUser.gid, currentEvalCycleId);

                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x, "While rejecting for " + reporteeId);

            }
            return Json(new { msg = "error" });
        }

        [HttpPost]
        public JsonResult getHRrejectionmessage(long evalcycleid, string employeeid)
        {
            Guid employeeId = Guid.Parse(employeeid);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == employeeId);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            var rejectedMessages = db.RejectedMessages.Where(x => x.ManagerID == currentUser.gid &&
                    x.EvalCycleId == currentEvalCycleId && x.EvaluationPhase == PECycle.EVALUATION.ToString()
                    && x.EmployeeId == employeeId
                    ).ToList();
            if (rejectedMessages == null || rejectedMessages.Count == 0)
            {
                return null;
            }
            string rejectionMessages = "";
            rejectedMessages.ForEach(x => rejectionMessages += x.RejectionReason + "<br/>");
            rejectionMessages += "";
            return Json((object)rejectionMessages, JsonRequestBehavior.AllowGet);
        }

        public JsonResult rejectByHr(string employeeid, long evalcycleid, string reason)
        {
            Guid reportee_id = Guid.Parse(employeeid);
            try
            {
                new HrEvaluationService(OrgId, AppName,currentUser).RejectManagersEvaluation(reportee_id, evalcycleid, reason);
                new Mail().SendEmail(Mail.ACTION_TYPE.EVALUATION_REJECTED_BY_HR, reportee_id, currentUser.gid, evalcycleid);
                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x, "rejectByHr - While rejecting for " + employeeid);

            }
            return Json(new { msg = "error" });
        }

        public ActionResult SelfEvaluation()
        {
            Goal g = goalservice.GetGoals(currentUser.gid, CurrentEvalCycle.Id).FirstOrDefault(x => !x.Fixed);
            if (g != null && g.Status == GoalStatus.PUBLISHED)
                return GetSelfEvaluation(currentUser.gid.ToString(), CurrentEvalCycle.Id, false);
            else
                return GetSelfEvaluation(currentUser.gid.ToString(), CurrentEvalCycle.Id, true);
        }
        //
        // GET: /Evaluation/
        public ActionResult GetSelfEvaluation(string gid, long evalcycleid, bool selfevaldisplay = false)
        {
            Guid empid = Guid.Parse(gid);
            ViewBag.AttachmentForUser = gid.Trim();
            ManagerEvaluationService mgrService = new ManagerEvaluationService(OrgId, AppName,currentUser);
            evaluationViewModel.Goals = goalservice.GetGoals(empid, evalcycleid).OrderBy(x => x.Fixed).ThenBy(y => y.ModifiedDate).ToList();
            evaluationViewModel.SelfEvaluations = empEvalService.GetSelfEvaluations(empid, evalcycleid);
            evaluationViewModel.ManagerEvaluations = mgrService.GetReviews(empid, evalcycleid);
            evaluationViewModel.Rating = mgrService.CalculateAvgRating(empid, evalcycleid);
            //ViewBag.Rejected = 0;
            //if (CurrentEvalCycle != null)
            //{
            //    var rejectedMessages = db.RejectedMessages.Where(x => x.ManagerID == currentUser.gid &&
            //       x.EvalCycleId == CurrentEvalCycle.Id && x.EvaluationPhase == PECycle.EVALUATION.ToString()
            //       && x.EmployeeId == empid
            //       ).ToList();
            //    if (rejectedMessages.Count > 0)
            //    {
            //        ViewBag.Rejected = 1;
            //    }
            //}
            var c = evaluationViewModel.Goals.FirstOrDefault();
            if (c != null)
            {
                if (c.Status >= GoalStatus.EMPLOYEE_EVAL_PUBLISHED)
                {
                    evaluationViewModel.IsEditable = false;
                }
            }
            foreach (EmployeeEvaluation selfeval in evaluationViewModel.SelfEvaluations)
            {
                var attch = empEvalService.GetAttachments(selfeval.Id);
                if (attch != null && attch.Count > 0)
                    evaluationViewModel.Attachments.AddRange(attch);
            }
            if (selfevaldisplay)
                return View("SelfEvaluation", evaluationViewModel);
            else
            {
                evaluationViewModel.Feedbackanswers = db.FeedbackAnswers.Where(x => x.EmployeeId == empid && x.EvalCycleId == evalcycleid).ToList();
                evaluationViewModel.FeedbackQuestions = db.FeedbackQuestions.Where(y => y.OrgId == OrgId).OrderBy(o => o.QuestionId).ToList();
                evaluationViewModel.Conclusion = db.EvaluationConclusions.FirstOrDefault(x => x.employeeid == empid && x.evalcycleid == evalcycleid) ?? new EvaluationConclusion();
                foreach (FeedbackQuestion fq in evaluationViewModel.FeedbackQuestions)
                {
                    evaluationViewModel.Options.AddRange(db.FeedbackAnswerOptions.Where(x => x.QuestionId == fq.QuestionId).ToList());
                }
                ViewBag.ShowFeedback = false;
                ViewBag.ShowSelfRating = false;
                ViewBag.Evalcycleid = evalcycleid;
                return View("HrEvaluation", evaluationViewModel);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveSelfEvaluation(string goalid, string selfeval, string selfrating)
        {
            try
            {
                //int count = 0;
                //var gs = db.Goals.Where(x => !x.Fixed && x.EmployeeId == currentUser.gid && x.Evalcycleid == CurrentEvalCycle.Id && x.Status.Value == GoalStatus.EMPLOYEE_EVAL_SAVED);
                //if (gs != null)
                //    count = gs.Count();
                //if (count == 0)
                //    new Mail().SendEmail(Mail.ACTION_TYPE.SUBMIT_EVALUATION_TO_MANAGER, currentUser.gid, CurrentEvalCycle.Id);

                empEvalService.DraftSelfEvaluation(selfeval, Guid.Parse(goalid), int.Parse(selfrating), currentUser.gid, CurrentEvalCycle.Id);
                EvaluationViewModel viewmodel = new EvaluationViewModel();
                viewmodel.Rating = new ManagerEvaluationService(OrgId, AppName,currentUser).CalculateAvgRating(currentUser.gid, CurrentEvalCycle.Id);
                return Json(new { msg = "success", viewmodel = viewmodel });
            }
            catch (Exception x)
            {
                LogError(x, "Error while saving self rating : " + selfeval + " -- " + selfrating + "  --- " + goalid);


            }
            return Json(new { msg = "error" });
        }
        public JsonResult PublishSelfEvaluation(int SelfOverallRating)
        {
            try
            {
                EvaluationRating e = db.EvaluationRatings.FirstOrDefault(x => x.EmpId == currentUser.gid && x.EvalCycleId == CurrentEvalCycle.Id);
                e.SelfOverallRating = SelfOverallRating;
                db.SaveChanges();
                empEvalService.PublishSelfEvaluation(currentUser.gid, CurrentEvalCycle.Id);
                new Mail().SendEmail(Mail.ACTION_TYPE.EVALUATE_REPORTEE,currentUser.Manager.Value, currentUser.gid, CurrentEvalCycle.Id);


                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x, "While publishing Self Eval");

            }
            return Json(new { msg = "error" });
        }
        public JsonResult saveoverallreviewcomment(string overallcomment, Guid empid, int overallrating)
        {
            try
            {
                var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empid);
                var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
                ManagerEvaluationService mes = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
                mes.SaveFinalReviewComment(overallcomment, empid, currentEvalCycleId);
                var rating = db.EvaluationRatings.Where(x => x.EmpId == empid && x.EvalCycleId == currentEvalCycleId).FirstOrDefault();
                rating.ManagerOverllRating = overallrating;
                db.SaveChanges();
            }
            catch(Exception x) {
                LogError(x, "Saving overall comment for gid= "+empid+"- "+x.Message);
                return Json(new { status = false, msg = x.Message });
            }
            return Json(new { status = true, msg = "" });
        }

        public JsonResult publish(string reporteeId)
        {
            Guid reportee_id = Guid.Parse(reporteeId);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            //try
            //{

            //    var rejectedMessages = db.RejectedMessages.Where(x => x.ManagerID == currentUser.gid &&
            //    x.EvalCycleId == CurrentEvalCycle.Id && x.EvaluationPhase == PECycle.EVALUATION.ToString()
            //    && x.EmployeeId == reportee_id
            //    ).ToList();
            //    if (rejectedMessages == null || rejectedMessages.Count == 0)
            //    {
            //        db.RejectedMessages.RemoveRange(rejectedMessages);
            //    }

            //}
            //catch (Exception x)
            //{
            //    logger.Error(x);
            //}
            try
            {
                var evalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
                new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).Publish(reportee_id, evalCycleId);
                new Mail().SendEmail(Mail.ACTION_TYPE.SUBMIT_FEEDBACK, reportee_id, currentUser.gid, evalCycleId);
                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x);

            }
            return Json(new { msg = "error" });
        }

        public ActionResult feedback(string eid, string evalcycleid)
        {
            return View();
        }

        public ActionResult ReporteeEvaluation(string reporteeId)
        {
            Guid reportee_id = Guid.Parse(reporteeId);
            var employeeOfInterest =  db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            goalservice = new GoalService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            empEvalService = new EmployeeEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            ViewBag.AttachmentForUser = reporteeId.Trim();
            var evalcycleid = ((EmployeeEvaluationService)empEvalService).GetEvalCycle().Id;
            evaluationViewModel.EmployeeDetails = employeeOfInterest;
            evaluationViewModel.Goals = goalservice.GetGoals(reportee_id, evalcycleid).OrderBy(x => x.Fixed).ThenBy(y => y.ModifiedDate).ToList();
            evaluationViewModel.SelfEvaluations = empEvalService.GetSelfEvaluations(reportee_id, evalcycleid).Where(x => x.Comment != null && x.Comment.Length > 0).ToList();
            evaluationViewModel.ManagerEvaluations = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetReviews(reportee_id, evalcycleid);
            evaluationViewModel.Rating = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).CalculateAvgRating(reportee_id, evalcycleid);

            //var goalsList = evaluationViewModel.Goals;
            //List<Guid> goalsToRemove = new List<Guid>();
            //foreach (Goal goal in evaluationViewModel.Goals)
            //{
            //    if (!evaluationViewModel.SelfEvaluations.Any(x => x.GoalId == goal.gid))
            //    {
            //        goalsToRemove.Add(goal.gid);
            //    }
            //}
            //foreach (Guid gid in goalsToRemove)
            //{
            //    goalsList.RemoveAll(x => x.gid == gid);
            //}
            //evaluationViewModel.Goals = goalsList;
            foreach (EmployeeEvaluation selfeval in evaluationViewModel.SelfEvaluations)
            {
                var attch = empEvalService.GetAttachments(selfeval.Id);
                if (attch != null && attch.Count > 0)
                    evaluationViewModel.Attachments.AddRange(attch);
            }
            foreach (ManagerEvaluation mgrEval in evaluationViewModel.ManagerEvaluations)
            {
                var attch = empEvalService.GetAttachments(mgrEval.Id);
                if (attch != null && attch.Count > 0)
                    evaluationViewModel.Attachments.AddRange(attch);
            }
            var c = evaluationViewModel.Goals.FirstOrDefault();
            if (c != null)
            {
                if (c.Status == GoalStatus.PUBLISHED || c.Status == GoalStatus.MANAGER_EVAL_PUBLISHED)
                {
                    evaluationViewModel.IsEditable = false;

                }
            }
            if (currentUser.gid.ToString() != evaluationViewModel.EmployeeDetails.Manager.ToString())
                evaluationViewModel.IsEditable = false;

            evaluationViewModel.Conclusion = db.EvaluationConclusions.FirstOrDefault(x => x.employeeid == reportee_id && x.evalcycleid == evalcycleid);
            return View("ManagerEvaluation", evaluationViewModel);
        }

        public JsonResult savesummary(string summary, string empid)
        {
            Guid reportee_id = Guid.Parse(empid);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            var conc = db.EvaluationConclusions.FirstOrDefault(x => x.employeeid == reportee_id && x.evalcycleid == currentEvalCycleId);
            if (conc == null)
            {
                conc = new EvaluationConclusion();
                conc.employeeid = reportee_id;
                conc.evalcycleid = currentEvalCycleId;
                conc.gid = Guid.NewGuid();
                conc.meetingsummary = summary;
                conc.modifiedon = DateTime.Today;
                db.EvaluationConclusions.Add(conc);
            }
            else
            {
                conc.meetingsummary = summary;
                conc.modifiedon = DateTime.Today;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception x)
            {
                LogError(x, "While saving summary for " + empid);
                return Json(new { status = false });
            }
            return Json(new { status = true });
        }


        public JsonResult savetraining(string training, string empid)
        {
            Guid reportee_id = Guid.Parse(empid);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            var conc = db.EvaluationConclusions.FirstOrDefault(x => x.employeeid == reportee_id && x.evalcycleid == currentEvalCycleId);
            if (conc == null)
            {
                conc = new EvaluationConclusion();
                conc.employeeid = reportee_id;
                conc.evalcycleid = currentEvalCycleId;
                conc.gid = Guid.NewGuid();
                conc.training = training;
                conc.modifiedon = DateTime.Today;
                db.EvaluationConclusions.Add(conc);
            }
            else
            {
                conc.training = training;
                conc.modifiedon = DateTime.Today;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception x)
            {
                LogError(x, "While saving training for " + empid);
                return Json(new { status = false });
            }
            return Json(new { status = true });
        }
        [ValidateInput(false)]
        public JsonResult SaveReporteeEvaluation(string reporteeid, string goalid, string reviewcomment, string reviewrating)
        {
            try
            {
                Guid reportee_id = Guid.Parse(reporteeid);
                var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
                //int count = 0;
                //var gs = db.Goals.Where(x => !x.Fixed && x.EmployeeId == reportee_id && x.Evalcycleid == CurrentEvalCycle.Id && x.Status.Value == GoalStatus.MANAGER_EVAL_SAVED);
                //if (gs != null)
                //    count = gs.Count();
                //if (count == 0)
                //    new Mail().SendEmail(Mail.ACTION_TYPE.SEND_EVALUATION_TO_HR, currentUser.gid, CurrentEvalCycle.Id);

                ManagerEvaluationService mgrEvalService = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
                var evalcycleid = mgrEvalService.GetEvalCycle().Id;
                EvaluationViewModel viewmodel = new EvaluationViewModel();
                mgrEvalService.DraftReview(Guid.Parse(goalid), currentUser.gid, reviewcomment, int.Parse(reviewrating), reportee_id, evalcycleid);

                viewmodel.Rating = viewmodel.Rating = new ManagerEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).CalculateAvgRating(reportee_id, evalcycleid);

                return Json(new { msg = "success", viewmodel = viewmodel });
            }
            catch (Exception x)
            {
                LogError(x, "Error while saving self rating : " + reviewcomment + " -- " + reviewrating + "  --- " + goalid);

            }
            return Json(new { msg = "error" });
        }
        public JsonResult PublishReporteeEvaluation(string reporteeId)
        {
            Guid reportee_id = Guid.Parse(reporteeId);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            try
            {
               
                new Mail().SendEmail(Mail.ACTION_TYPE.APPROVE_EVALUATIONS, reportee_id, currentUser.gid, currentEvalCycleId);

                var rejectedMessages = db.RejectedMessages.Where(x => x.ManagerID == currentUser.gid &&
                x.EvalCycleId == currentEvalCycleId && x.EvaluationPhase == PECycle.EVALUATION.ToString()
                && x.EmployeeId == reportee_id
                ).ToList();
                if (rejectedMessages != null && rejectedMessages.Count > 0)
                {
                    db.RejectedMessages.RemoveRange(rejectedMessages);
                    db.SaveChanges();
                }

            }
            catch (Exception x)
            {
                LogError(x);
            }
            try
            {



                new ManagerEvaluationService(OrgId, AppName, currentUser).PublishReview(reportee_id, currentEvalCycleId);
                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x);

            }
            return Json(new { msg = "error" });
        }
        public ActionResult EmployeeEvaluation()
        {
            return View();
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult FileUpload(string goalid, string empid, string evaltype = EvaluationType.SELF_EVALUATION)
        {
            Guid reportee_id = Guid.Parse(empid);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            List<Attachment> attachments = new List<Attachment>();
            if (string.IsNullOrEmpty(goalid))
                return new EmptyResult();
            if (goalid != null)
            {
                ViewBag.AttachmentForUser = empid;
                ViewBag.goalid = goalid;
                ViewBag.evaltype = evaltype;
                byte[] fileBytes = null;
                Guid gid = Guid.Parse(goalid);
                if (evaltype == EvaluationType.SELF_EVALUATION)
                {
                    var empeval = db.EmployeeEvaluations.FirstOrDefault(x => x.GoalId == gid && x.EmployeeId == currentUser.gid && x.EvalCycleId == CurrentEvalCycle.Id);
                    if (empeval != null)
                    {
                        var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == empeval.Id);
                        if (allattachments != null)
                            attachments = allattachments.ToList();
                    }
                }
                else if (evaltype == EvaluationType.MANAGER_EVALUATION)
                {
                    var mgrEval = db.ManagerEvaluations.FirstOrDefault(x => x.GoalId == gid && x.EvalCycleId == currentEvalCycleId);
                    if (mgrEval != null)
                    {
                        var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == mgrEval.Id);
                        if (allattachments != null)
                            attachments = allattachments.ToList();
                    }
                }
            }
            return PartialView("Attachments", attachments);
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public JsonResult GetAllAttachedFiles(string goalid)
        {
            List<Attachment> attachments = new List<Attachment>();
            if (goalid != null)
            {
                ViewBag.goalid = goalid;
                byte[] fileBytes = null;
                Guid gid = Guid.Parse(goalid);

                var empeval = db.EmployeeEvaluations.FirstOrDefault(x => x.GoalId == gid && x.EvalCycleId == CurrentEvalCycle.Id);
                if (empeval != null)
                {
                    var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == empeval.Id);
                    if (allattachments != null)
                        attachments = allattachments.ToList();
                }
            }
            return Json(attachments, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult AttachmentList(string gid, string goalid, string evaltype = EvaluationType.SELF_EVALUATION)
        {
            Guid? empid = new Guid();
            if (!string.IsNullOrEmpty(gid))
            {
                empid = Guid.Parse(gid);
            }
            else
            {
                return new EmptyResult();
            }
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empid);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            List<Attachment> attachments = new List<Attachment>();
            if (goalid != null)
            {
                ViewBag.goalid = goalid;
                ViewBag.evaltype = evaltype;
                byte[] fileBytes = null;
                Guid goal_id = Guid.Parse(goalid);

                if (evaltype == EvaluationType.SELF_EVALUATION)
                {
                    var empeval = db.EmployeeEvaluations.FirstOrDefault(x => x.GoalId == goal_id && x.EmployeeId == empid && x.EvalCycleId == currentEvalCycleId);
                    if (empeval != null)
                    {
                        var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == empeval.Id);
                        if (allattachments != null)
                            attachments = allattachments.ToList();
                    }
                }
                else if (evaltype == EvaluationType.MANAGER_EVALUATION)
                {
                    var mgrEval = db.ManagerEvaluations.FirstOrDefault(x => x.GoalId == goal_id && x.EmployeeId == empid && x.EvalCycleId == currentEvalCycleId);
                    if (mgrEval != null)
                    {
                        var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == mgrEval.Id);
                        if (allattachments != null)
                            attachments = allattachments.ToList();
                    }
                }
            }
            return PartialView(attachments);
        }

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/attachments")); }
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }


        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        public JsonResult FileUpload(HttpPostedFileBase files, string empid, string goalid, string evaltype = EvaluationType.SELF_EVALUATION)
        {
            Guid goal_id = Guid.Parse(goalid);
            Guid empId = Guid.Parse(empid);
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empId);
            var currentEvalCycleId = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest).GetEvalCycle().Id;
            Guid evaluationId = new Guid();
            int totalAttachments = 0;
            var fullPath = Path.Combine(StorageRoot, goalid + "_" + Path.GetFileName(files.FileName));
            files.SaveAs(fullPath);
            if (evaltype == EvaluationType.SELF_EVALUATION)
            {
                var empeval = db.EmployeeEvaluations.FirstOrDefault(x => x.EmployeeId == empId && x.GoalId == goal_id && x.EvalCycleId == currentEvalCycleId);
                var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == empeval.Id);
                if (empeval != null)
                    evaluationId = empeval.Id;
                if (allattachments != null)
                    totalAttachments = allattachments.Count();
            }
            else if (evaltype == EvaluationType.MANAGER_EVALUATION)
            {
                var mgrEval = db.ManagerEvaluations.FirstOrDefault(x => x.EmployeeId == empId && x.GoalId == goal_id && x.EvalCycleId == currentEvalCycleId);
                var allattachments = db.Attachments.Where(x => x.EvaluationGoalId == mgrEval.Id);
                if (mgrEval != null)
                    evaluationId = mgrEval.Id;
                if (allattachments != null)
                    totalAttachments = allattachments.Count();
            }
            if (totalAttachments >= 5)
            {
                //remove later
                return Json(new
                {

                    files = new ArrayList(){ new {
                                                    name=files.FileName,
                                                    size = files.ContentLength,
                                                    url = "",
                                                    deleteUrl = "",
                                                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(Path.Combine(Server.MapPath("~/images/Location-File.png"))),
                                                    deleteType = "GET",
                                                     error= "Max 5 attachments allowed."
                                            }}

                });
            }
            Guid attachmentGuid = new Guid();

            if (evaluationId.ToString() != new Guid().ToString())
            {
                try
                {
                    byte[] documentBytes = new byte[files.InputStream.Length];
                    files.InputStream.Read(documentBytes, 0, documentBytes.Length);

                    attachmentGuid = empEvalService.SaveAttachment(evaluationId, documentBytes, files.FileName, filetype(Path.GetExtension(files.FileName)));
                    var attachment = db.Attachments.Where(x => x.AttachmentId == attachmentGuid).FirstOrDefault();
                    attachment.EvaluationType = evaltype;
                    db.SaveChanges();

                }
                catch (ArgumentException argex)
                {
                    logger.Error("Error while saving attachment : " + files.FileName + " -- " + goalid, argex);
                    return Json(new
                    {

                        files = new ArrayList(){ new {
                                                    name=files.FileName,
                                                    size = files.ContentLength,
                                                    url = Url.Content("~/")+"evaluation/Download/?fileid=" +  attachmentGuid.ToString(),
                                                    deleteUrl = Url.Content("~/")+"evaluation/Delete/?fileid=" +  attachmentGuid.ToString(),
                                                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(Path.Combine(Server.MapPath("~/images/Location-File.png"))),
                                                    deleteType = "GET",
                                                    error="This file is not supported."
                                            }}

                    });
                }
                catch (InvalidOperationException ioex)
                {
                    return Json(new
                    {

                        files = new ArrayList(){ new {
                                                    name=files.FileName,
                                                    size = files.ContentLength,
                                                    url = Url.Content("~/")+"evaluation/Download/?fileid=" +  attachmentGuid.ToString(),
                                                    deleteUrl = Url.Content("~/")+"evaluation/Delete/?fileid=" +  attachmentGuid.ToString(),
                                                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(Path.Combine(Server.MapPath("~/images/Location-File.png"))),
                                                    deleteType = "GET",
                                                    error="This file is not supported."
                                            }}

                    });
                }
                catch (Exception ex)
                {
                    LogError(ex, "Error while saving attachment : " + files.FileName + " -- " + goalid);
                    return Json(new
                    {

                        files = new ArrayList(){ new {
                                                    name=files.FileName,
                                                    size = files.ContentLength,
                                                    url = Url.Content("~/")+"evaluation/Download/?fileid=" +  attachmentGuid.ToString(),
                                                    deleteUrl = Url.Content("~/")+"evaluation/Delete/?fileid=" +  attachmentGuid.ToString(),
                                                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(Path.Combine(Server.MapPath("~/images/Location-File.png"))),
                                                    deleteType = "GET",
                                                    error="Unable to save attachment."
                                            }}

                    });
                }
            }


            return Json(new
                {

                    files = new ArrayList(){ new {
                                                    name=files.FileName,
                                                    size = files.ContentLength,
                                                    url = Url.Content("~/")+"evaluation/Download/?fileid=" +  attachmentGuid.ToString(),
                                                    deleteUrl = Url.Content("~/")+"evaluation/Delete/?fileid=" +  attachmentGuid.ToString(),
                                                    thumbnailUrl = @"data:image/png;base64," + EncodeFile(Path.Combine(Server.MapPath("~/images/Location-File.png"))),
                                                    deleteType = "GET",
                                            }}

                });
        }
        public void tooglefeedbackvisibility(long evalCycleId, Guid empId, bool isprivate, string fbtext)
        {
            EmployeeService es = new EmployeeService(OrgId, AppName, db.Employees.FirstOrDefault(x => x.gid == empId));
            es.UpdateActiveFeedbackVisibility(empId, evalCycleId, !isprivate,fbtext);
            return;
        }
        public ActionResult ActiveFeedback(Guid empId, long pecycle)
        {
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empId);
            ViewBag.IsSelf = empId == currentUser.gid;
            EmployeeService es = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            List<EmployeeService.OnGoingFeedback> fb = es.GetOnGoingFeedbackFor(empId,pecycle);
            empEvalService = new EmployeeEvaluationService(employeeOfInterest.OrgLocationId.Value, AppName, employeeOfInterest);
            ViewBag.AllCycles = empEvalService.GetAllCycles(empId);
            Hashtable ht = es.GetPECycleStatus(empId);
            if (ht.Count > 0)
            {
                var en = ht.Values.GetEnumerator();
                en.MoveNext();
                ViewBag.CurrentCycle = en.Current;
            }
            else
                ViewBag.CurrentCycle = new EvaluationCycle() { Id = 0 };

            ViewBag.Employee = employeeOfInterest;
            ViewBag.pecycleId = pecycle;
            ViewBag.employeeid = empId;
            //ViewBag.AllowEditing = (!ViewBag.IsSelf && (ViewBag.pecycleId == ViewBag.CurrentCycle.Id || ViewBag.pecycleId == )) ;

            return View(fb!=null ? fb.OrderByDescending(x => x.FeedbackDate).ToList():fb);
        }
        public void saveactivefeedback(long evalCycleId, string feedback, Guid empId, bool isprivate)
        {
            var employeeOfInterest = db.Employees.FirstOrDefault(x => x.gid == empId);
            EmployeeService es = new EmployeeService(employeeOfInterest.OrgLocationId.Value, AppName, currentUser);
            es.SaveOnGoingFeedbackFor(empId, evalCycleId, feedback, isprivate);
            return;
        }
        private string filetype(string ext)
        {

            //Set the contenttype based on File Extension
            string contenttype = "";
            switch (ext.ToLower())
            {
                case ".zip":

                    contenttype = "application/zip";

                    break;
                case ".rar":

                    contenttype = "application/x-rar-compressed";

                    break;

                case ".doc":

                    contenttype = "application/vnd.ms-word";

                    break;

                case ".docx":

                    contenttype = "application/vnd.ms-word";

                    break;

                case ".xls":

                    contenttype = "application/vnd.ms-excel";

                    break;

                case ".xlsx":

                    contenttype = "application/vnd.ms-excel";

                    break;

                case ".jpg":
                case ".jpeg":
                    contenttype = System.Net.Mime.MediaTypeNames.Image.Jpeg;

                    break;

                case ".png":

                    contenttype = "image/png";

                    break;

                case ".gif": ;

                    contenttype = System.Net.Mime.MediaTypeNames.Image.Gif;

                    break;

                case ".pdf":

                    contenttype = System.Net.Mime.MediaTypeNames.Application.Pdf;

                    break;
                case ".msg":
                    contenttype = "application/vnd.ms-outlook";
                    break;

            }
            if (contenttype == "")
                throw new ArgumentException("Unsupported file ! Incoming file extension : " + ext);
            return contenttype;
        }


        public ActionResult Download(string fileid)
        {
            Guid attachmentId = Guid.Parse(fileid);
            Attachment att = db.Attachments.FirstOrDefault(x => x.AttachmentId == attachmentId);
            if (att != null)
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    // for example foo.bak
                    FileName = HttpUtility.UrlEncode(att.FileName),

                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(att.FileContents, att.FileType);
            }
            else return new HttpStatusCodeResult(404);

        }

        [HttpGet]
        public JsonResult Delete(string fileid)
        {


            //var filePath = Path.Combine(Server.MapPath("~/attachments"), filename);

            //if (System.IO.File.Exists(filePath))
            //{
            //    System.IO.File.Delete(filePath);
            //}
            Guid attachmentId = Guid.Parse(fileid);
            string filename = db.Attachments.FirstOrDefault(x => x.AttachmentId == attachmentId).FileName;
            empEvalService.DeleteAttachment(attachmentId);
            var f = "{'files': '" + attachmentId + "'}";

            return Json((object)f, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeeEvaluationStatus()
        {
            long evalcycleid = 0;
            List<EmployeeEvaluationListModel> model = new List<EmployeeEvaluationListModel>();
            EmployeeEvaluationListModel viewmodel = new EmployeeEvaluationListModel();
            if (CurrentGoalCycle != null)
            {
                evalcycleid = CurrentGoalCycle.Id;
                viewmodel.PEType = PECycle.GOAL_SETTING;
                viewmodel.PECycleId = evalcycleid;
                viewmodel.PECycleTitle = CurrentGoalCycle.Title;
                GetEmployeesEvaluationList(ref viewmodel, evalcycleid);
                if (viewmodel.EmployeeEvaluationStatus != null)
                    model.Add(viewmodel);
            }
            if (CurrentEvalCycle != null)
            {
                viewmodel = new EmployeeEvaluationListModel();
                evalcycleid = CurrentEvalCycle.Id;
                viewmodel.PEType = PECycle.EVALUATION;
                viewmodel.PECycleId = evalcycleid;
                viewmodel.PECycleTitle = CurrentEvalCycle.Title;
                GetEmployeesEvaluationList(ref viewmodel, evalcycleid);
                if (viewmodel.EmployeeEvaluationStatus != null)
                    model.Add(viewmodel);
            }


            return View(model);
        }
        private void GetEmployeesEvaluationList(ref EmployeeEvaluationListModel viewmodel, long evalcycleid)
        {
            if (evalcycleid > 0)
            {
                if (db.Goals.Any(x => x.Evalcycleid == evalcycleid && !x.Fixed))
                {
                    var listOfEmployees = db.Goals.Where(x => x.Evalcycleid == evalcycleid && !x.Fixed).Select(x => x.EmployeeId).Distinct().ToList();
                    viewmodel.EmployeeEvaluationStatus = new List<EmployeeEvaluationStatusModel>();
                    foreach (Guid g in listOfEmployees)
                    {
                        var Emp = db.Employees.FirstOrDefault(x => x.gid == g && x.Active==true);
                        if (Emp == null)
                        {
                            continue;
                        }
                        var manager = db.Employees.FirstOrDefault(x => x.gid == Emp.Manager.Value);
                        if (manager == null)
                            manager = new Employee();
                        viewmodel.EmployeeEvaluationStatus.Add(new EmployeeEvaluationStatusModel()
                        {
                            EmployeeId = g,
                            ManagerId = Emp.Manager.HasValue ? Emp.Manager.Value : new Guid(),
                            ManagerName = manager.FirstName ?? "" + " " + manager.LastName ?? "",
                            Name = Emp.FirstName + " " + Emp.LastName,
                            EmpOrgId = Emp.OrgEmpId ?? "",
                            Status = db.Goals.Any(x => x.Evalcycleid == evalcycleid && x.EmployeeId == g) ? db.Goals.First(x => x.Evalcycleid == evalcycleid && x.EmployeeId == g).Status.Value : GoalStatus.NOT_STARTED
                        });
                    }
                    viewmodel.EmployeeEvaluationStatus = viewmodel.EmployeeEvaluationStatus.OrderBy(x => x.Status).ToList();

                }
            }
        }

        public ActionResult HrEvaluation(string employeeid, long evalcycleid)
        {

            Guid reportee_id = Guid.Parse(employeeid);
            if (Impersonator != null)
            {
                return PartialView("Message", (object)"Sorry! Viewing this page is not permitted while impersonating. You will have to stop impersonating to view this.");
            }
            ViewBag.AttachmentForUser = employeeid.Trim();
            evaluationViewModel.EmployeeDetails = db.Employees.FirstOrDefault(x => x.gid == reportee_id);
            evaluationViewModel.Goals = goalservice.GetGoals(reportee_id, evalcycleid).OrderBy(x => x.Fixed).ThenBy(y => y.ModifiedDate).ToList();
            var tempgoal = evaluationViewModel.Goals.FirstOrDefault(x => !x.Fixed);
            int currentEmployeeGoalStatus = 0;
            if (tempgoal != null)
            {
                currentEmployeeGoalStatus = tempgoal.Status.Value;
            }
            var aspnetUser  = db.aspnet_Users.FirstOrDefault(x=>x.UserId==evaluationViewModel.EmployeeDetails.UserId);
            if (currentUser.Email.ToLower().Trim() != "nitasha.dusi@avalara.com"  &&
                    (
                        (
                            // if the current user is trying to view his/her eval before HR approval
                            reportee_id == currentUser.gid && currentEmployeeGoalStatus != GoalStatus.PUBLISHED &&
                            (User.IsInRole("Hr") || User.IsInRole("HrAdmin"))
                        ) ||
                (
                    // if the current user and employee are both HR AND the current user is NOT manager of employee then error out
                     Roles.GetRolesForUser(aspnetUser.UserName).Any(x=>x.ToUpperInvariant().Contains("HR")) 
                     &&  (User.IsInRole("Hr") || User.IsInRole("HrAdmin"))
                     && (evaluationViewModel.EmployeeDetails.Manager.HasValue?evaluationViewModel.EmployeeDetails.Manager.Value:Guid.Empty)!=currentUser.gid
                )
                        ||
                        reportee_id == currentUser.Manager
                    )
                )
            {
                ViewBag.ClassToApply = "alert alert-danger";
                return View("Message", (object)"Oh snap! Sorry, you are not authorized to view this.");
            }
            evaluationViewModel.SelfEvaluations = empEvalService.GetSelfEvaluations(reportee_id, evalcycleid);
            evaluationViewModel.ManagerEvaluations = new ManagerEvaluationService(OrgId, AppName,currentUser).GetReviews(reportee_id, evalcycleid);
            evaluationViewModel.Rating = new ManagerEvaluationService(OrgId, AppName,currentUser).CalculateAvgRating(reportee_id, evalcycleid);
            ViewBag.Evalcycleid = evalcycleid;
            foreach (EmployeeEvaluation selfeval in evaluationViewModel.SelfEvaluations)
            {
                var attch = empEvalService.GetAttachments(selfeval.Id);
                if (attch != null && attch.Count > 0)
                    evaluationViewModel.Attachments.AddRange(attch);
            }
            foreach (ManagerEvaluation mgrEval in evaluationViewModel.ManagerEvaluations)
            {
                var attch = empEvalService.GetAttachments(mgrEval.Id);
                if (attch != null && attch.Count > 0)
                    evaluationViewModel.Attachments.AddRange(attch);
            }

            evaluationViewModel.Feedbackanswers = db.FeedbackAnswers.Where(x => x.EmployeeId == reportee_id && x.EvalCycleId == evalcycleid).ToList();
            evaluationViewModel.FeedbackQuestions = db.FeedbackQuestions.Where(y => y.OrgId == OrgId).OrderBy(o => o.QuestionId).ToList();
            evaluationViewModel.Conclusion = db.EvaluationConclusions.FirstOrDefault(x => x.employeeid == reportee_id && x.evalcycleid == evalcycleid) ?? new EvaluationConclusion();
            foreach (FeedbackQuestion fq in evaluationViewModel.FeedbackQuestions)
            {
                evaluationViewModel.Options.AddRange(db.FeedbackAnswerOptions.Where(x => x.QuestionId == fq.QuestionId).ToList());
            }

            var c = evaluationViewModel.Goals.FirstOrDefault();
            if (c != null)
            {
                if (c.Status == GoalStatus.MANAGER_EVAL_PUBLISHED)
                {
                    evaluationViewModel.IsEditable = true;
                }
                else
                {
                    evaluationViewModel.IsEditable = false;
                }
            }

            return PartialView(evaluationViewModel);
        }

        public JsonResult PublishHrEvaluation(string employeeid, long evalcycleid)
        {
            try
            {
                Guid reportee_id = Guid.Parse(employeeid);
                new HrEvaluationService(OrgId, AppName,currentUser).PublishEvaluation(reportee_id, evalcycleid);
                new Mail().SendEmail(Mail.ACTION_TYPE.PUBLISH, reportee_id, currentUser.gid, evalcycleid);
                return Json(new { msg = "success" });
            }
            catch (Exception x)
            {
                LogError(x);

            }
            return Json(new { msg = "error" });
        }



        public ActionResult AllEvalCycles(string empid)
        {
            Guid? empId = Guid.Parse(empid);
            List<EvaluationCycle> evalcycles = empEvalService.GetAllCycles(empId);
            return PartialView(evalcycles);
        }

        public ActionResult ExportSelfEvaluation(string empid, long evalcycleid)
        {
            Guid gid = Guid.Parse(empid);
            string evalcyclename = "";
            var selfEvals = empEvalService.GetSelfEvaluations(gid, evalcycleid);
            var evalcycle = db.EvaluationCycles.FirstOrDefault(x => x.Id == evalcycleid);
            Employee emp = db.Employees.FirstOrDefault(x => x.gid == gid);
            if (evalcycleid != null)
                evalcyclename = evalcycle.Title;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + emp.FirstName + "_" + emp.LastName + "_SelfEvaluation_" + Server.HtmlEncode(evalcyclename) + ".xls\"");
            SelfEvalExportViewmodel viewmodel = new SelfEvalExportViewmodel() { Evalcycle = evalcycle, SelfEvals = selfEvals };
            Employee Manager = db.Employees.FirstOrDefault(x => x.gid == emp.Manager);
            if (Manager != null)
            {
                viewmodel.ManagerName = Manager.FirstName + " " + Manager.LastName;
            }

            viewmodel.Employee = emp;
            var org = db.OrgLocations.FirstOrDefault(x => x.Id == OrgId);
            if (org != null)
                viewmodel.OrgName = org.LocationName;
            viewmodel.Goals = db.Goals.Where(x => (x.Fixed || (x.EmployeeId == gid && x.Evalcycleid == evalcycleid)) ).OrderBy(z => z.Fixed).ThenBy(y => y.ModifiedDate).ToList();
            try
            {
                if (viewmodel.Goals.Where(x => !x.Fixed).FirstOrDefault().Status == GoalStatus.PUBLISHED || currentUser.gid.ToString() == Manager.gid.ToString())
                {
                    viewmodel.MgrEvals = empEvalService.GetManagersEvaluation(gid, evalcycleid);
                }
                viewmodel.GradingObject = new EmployeeEvaluationService(org.Id, AppName, currentUser).GetOverallEvaluationRating(gid, evalcycleid);
                if (viewmodel.GradingObject == null)
                {
                    viewmodel.GradingObject = new EvaluationRating();
                }
            }
            catch { }
            return View(viewmodel);

        }

       

    }
}