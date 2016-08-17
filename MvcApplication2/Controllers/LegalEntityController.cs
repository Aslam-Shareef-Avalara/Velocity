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
    public class LegalEntityController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: LegalEntity/Index
        public ActionResult Index()
        {
            return View(db.LegalEntities.ToList());
        }

        /*
        // GET: LegalEntity/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LegalEntity legalEntity = db.LegalEntities.Find(id);
            if (legalEntity == null)
            {
                return HttpNotFound();
            }
            return View(legalEntity);
        }
        */

        // GET: LegalEntity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LegalEntity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Vendors")] LegalEntity legalEntity)
        {
            if (ModelState.IsValid)
            {
                db.LegalEntities.Add(legalEntity);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a LegalEntity record");
                return RedirectToAction("Index");
            }

            DisplayErrorMessage();
            return View(legalEntity);
        }

        // GET: LegalEntity/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LegalEntity legalEntity = db.LegalEntities.Find(id);
            if (legalEntity == null)
            {
                return HttpNotFound();
            }
            return View(legalEntity);
        }

        // POST: LegalEntity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Vendors")] LegalEntity legalEntity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(legalEntity).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a LegalEntity record");
                return RedirectToAction("Index");
            }
            DisplayErrorMessage();
            return View(legalEntity);
        }

        // GET: LegalEntity/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LegalEntity legalEntity = db.LegalEntities.Find(id);
            if (legalEntity == null)
            {
                return HttpNotFound();
            }
            return View(legalEntity);
        }

        // POST: LegalEntity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            LegalEntity legalEntity = db.LegalEntities.Find(id);
            db.LegalEntities.Remove(legalEntity);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a LegalEntity record");
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
