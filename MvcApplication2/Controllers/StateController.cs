using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataService.InventoryManagement;


namespace MvcApplication2.Controllers
{
    public class StateController : BaseController
    {
        private InvMgmtEntities db1 = new InvMgmtEntities();

        // GET: State/Index
        public ActionResult Index()
        {
            var state = db1.States.Include(s => s.Country);
            return View(state.ToList());
        }

        /*
        // GET: State/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }
        */

        // GET: State/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db1.Countries, "Id", "Name");
            return View();
        }

        // POST: State/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CountryId,Country")] State state)
        {
            if (ModelState.IsValid)
            {
                db1.States.Add(state);
                db1.SaveChanges();
                DisplaySuccessMessage("Has append a State record");
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(db1.Countries, "Id", "Name", state.CountryId);
            DisplayErrorMessage();
            return View(state);
        }

        // GET: State/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db1.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db1.Countries, "Id", "Name", state.CountryId);
            return View(state);
        }

        // POST: State/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CountryId,Country")] State state)
        {
            if (ModelState.IsValid)
            {
                db1.Entry(state).State = EntityState.Modified;
                db1.SaveChanges();
                DisplaySuccessMessage("Has update a State record");
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db1.Countries, "Id", "Name", state.CountryId);
            DisplayErrorMessage();
            return View(state);
        }

        // GET: State/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db1.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // POST: State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            State state = db1.States.Find(id);
            db1.States.Remove(state);
            db1.SaveChanges();
            DisplaySuccessMessage("Has delete a State record");
            return RedirectToAction("Index");
        }

        private void DisplaySuccessMessage(string msgText)
        {
            TempData["SuccessMessage"] = msgText;
        }

        private void DisplayErrorMessage()
        {
            TempData["ErrorMessage"] = "Save changes was unsuccessful.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db1.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
