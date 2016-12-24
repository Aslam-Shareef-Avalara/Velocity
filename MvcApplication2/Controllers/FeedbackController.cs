using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using X.Scaffolding.Core;
using PagedList;
using DataService;
using MvcApplication2.Models;

namespace MvcApplication2.Controllers
{
    [Authorize]
    public class FeedbackController : BaseController
    {
        private PEntities db = new PEntities();

        [HttpPost]
        public ActionResult Index(string [] answers)
        {
            EvaluationCycle evalcycle = CurrentEvalCycle;
            if (evalcycle == null)
            {
                evalcycle = PrevEvalCycle;
                if (evalcycle == null)
                    return View("Message", "There is no feedback necessary at this time.");
            }
            FeedbackAnswer [] feedback = new  FeedbackAnswer[Request.Form.Keys.Count];
            for (int i = 0; i < Request.Form.Keys.Count;i++ )
            {
                int questionid = int.Parse(Request.Form.Keys[i].Replace("answer_", ""));
                string answer = Request.Form[Request.Form.Keys[i]];
                feedback[i] = new FeedbackAnswer()
                {
                     Answer = answer,
                     EmployeeId = currentUser.gid,
                     EvalCycleId = evalcycle.Id,
                     QuestionId = questionid,
                     AnswerId = Guid.NewGuid()
                };
            }
            db.FeedbackAnswers.AddRange(feedback);
            db.SaveChanges();

            FeedbackModel model = new FeedbackModel();
            model.Evalcycle = evalcycle;
            
            model.Answers = db.FeedbackAnswers.Where(x => x.EmployeeId == currentUser.gid && x.EvalCycleId == evalcycle.Id).ToList();
            model.Questions = db.FeedbackQuestions.Where(y => y.OrgId == OrgId).OrderBy(o => o.QuestionId).ToList();
            foreach (FeedbackQuestion fq in model.Questions)
            {
                model.Options.AddRange(db.FeedbackAnswerOptions.Where(x => x.QuestionId == fq.QuestionId).ToList());
            }
            model.IsEditable = false;
            return View(model);
        }

        public ActionResult Index()
        {
            EvaluationCycle evalcycle = CurrentEvalCycle;
            FeedbackModel model = new FeedbackModel();
            EmployeeEvaluationService ees =   new EmployeeEvaluationService(OrgId, AppName,currentUser);
            
            if (evalcycle == null)
            {
                evalcycle = PrevEvalCycle;
                if (evalcycle == null)
                    return View("Message", (object)"There is no feedback necessary at this time.");
            }
            if (!ees.IsEvaluationComplete(currentUser.gid, evalcycle.Id))
            {
                return View("Message", (object)"There is no feedback necessary at this time.");
            }
            if (!db.FeedbackAnswers.Any(x => x.EmployeeId == currentUser.gid && x.EvalCycleId == evalcycle.Id))
            {
                model.IsEditable = true;
            }
            else
            {
                model.IsEditable = false;
                model.Answers = db.FeedbackAnswers.Where(x => x.EmployeeId == currentUser.gid && x.EvalCycleId == evalcycle.Id).ToList();
            }

            model.Evalcycle = evalcycle;
            model.Questions = db.FeedbackQuestions.Where(y=>y.OrgId==OrgId).OrderBy(o => o.QuestionId).ToList();
            foreach (FeedbackQuestion fq in model.Questions)
            {
                model.Options.AddRange(db.FeedbackAnswerOptions.Where(x => x.QuestionId == fq.QuestionId).ToList());
            }
            return View(model);
        }

        // GET: /Feedback/
        public ActionResult Questions(int? page)
        {

            return View("Questions", db.FeedbackQuestions.OrderBy(o => o.QuestionId).ToPagedList((page ?? 1), Paging.PageSize));
        }

        // GET: /Feedback/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackQuestion feedbackQuestion = db.FeedbackQuestions.Find(id);
            if (feedbackQuestion == null)
            {
                return HttpNotFound();
            }
            return View(feedbackQuestion);
        }

        // GET: /Feedback/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Feedback/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="QuestionId,Question,OrgId")] FeedbackQuestion feedbackQuestion)
        {
            if (ModelState.IsValid)
            {
                db.FeedbackQuestions.Add(feedbackQuestion);
                db.SaveChanges();
                return RedirectToAction("Questions");
            }

            return View(feedbackQuestion);
        }

        // GET: /Feedback/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackQuestion feedbackQuestion = db.FeedbackQuestions.Find(id);
            if (feedbackQuestion == null)
            {
                return HttpNotFound();
            }
            return View(feedbackQuestion);
        }

        // POST: /Feedback/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="QuestionId,Question,OrgId")] FeedbackQuestion feedbackQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedbackQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Questions");
            }
            return View(feedbackQuestion);
        }

        // GET: /Feedback/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedbackQuestion feedbackQuestion = db.FeedbackQuestions.Find(id);
            if (feedbackQuestion == null)
            {
                return HttpNotFound();
            }
            return View(feedbackQuestion);
        }

        // POST: /Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FeedbackQuestion feedbackQuestion = db.FeedbackQuestions.Find(id);
            db.FeedbackQuestions.Remove(feedbackQuestion);
            db.SaveChanges();
            return RedirectToAction("Questions");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
