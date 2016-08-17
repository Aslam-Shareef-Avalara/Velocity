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
    public class AssetIssueController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: AssetIssue/Index
        public ActionResult Index()
        {
            var assetIssue = db.AssetIssues.Include(a => a.Asset);
            return View(assetIssue.ToList());
        }

        /*
        // GET: AssetIssue/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetIssue assetIssue = db.AssetIssues.Find(id);
            if (assetIssue == null)
            {
                return HttpNotFound();
            }
            return View(assetIssue);
        }
        */

        // GET: AssetIssue/Create
        public ActionResult Create()
        {
            ViewBag.AssetId = new SelectList(db.Assets, "Id", "AssetTag");
            return View();
        }

        // POST: AssetIssue/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AssetId,DateIssued,IssuedTo,Comment,IssuedBy,Asset")] AssetIssue assetIssue)
        {
            if (ModelState.IsValid)
            {
                db.AssetIssues.Add(assetIssue);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a AssetIssue record");
                return RedirectToAction("Index");
            }

            ViewBag.AssetId = new SelectList(db.Assets, "Id", "AssetTag", assetIssue.AssetId);
            DisplayErrorMessage();
            return View(assetIssue);
        }

        // GET: AssetIssue/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetIssue assetIssue = db.AssetIssues.Find(id);
            if (assetIssue == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssetId = new SelectList(db.Assets, "Id", "AssetTag", assetIssue.AssetId);
            return View(assetIssue);
        }

        // POST: AssetIssue/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AssetId,DateIssued,IssuedTo,Comment,IssuedBy,Asset")] AssetIssue assetIssue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assetIssue).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a AssetIssue record");
                return RedirectToAction("Index");
            }
            ViewBag.AssetId = new SelectList(db.Assets, "Id", "AssetTag", assetIssue.AssetId);
            DisplayErrorMessage();
            return View(assetIssue);
        }

        // GET: AssetIssue/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetIssue assetIssue = db.AssetIssues.Find(id);
            if (assetIssue == null)
            {
                return HttpNotFound();
            }
            return View(assetIssue);
        }

        // POST: AssetIssue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            AssetIssue assetIssue = db.AssetIssues.Find(id);
            db.AssetIssues.Remove(assetIssue);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a AssetIssue record");
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
