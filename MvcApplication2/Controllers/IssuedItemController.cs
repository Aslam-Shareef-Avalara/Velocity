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
    public class IssuedItemController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: IssuedItem/Index
        public ActionResult Index()
        {
            var issuedItem = db.IssuedItems.Include(i => i.ItemMaster);
            return View(issuedItem.ToList());
        }

        /*
        // GET: IssuedItem/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            return View(issuedItem);
        }
        */

        // GET: IssuedItem/Create
        public ActionResult Create()
        {
            ViewBag.ItemMasterId = new SelectList(db.ItemMasters, "Id", "Name");
            return View();
        }

        // POST: IssuedItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemMasterId,IssuedOn,Quantity,IssuedBy,IssuedTo,OrgId,ItemMaster")] IssuedItem issuedItem)
        {
            if (ModelState.IsValid)
            {
                issuedItem.Id = Guid.NewGuid();
                db.IssuedItems.Add(issuedItem);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a IssuedItem record");
                return RedirectToAction("Index");
            }

            ViewBag.ItemMasterId = new SelectList(db.ItemMasters, "Id", "Name", issuedItem.ItemMasterId);
            DisplayErrorMessage();
            return View(issuedItem);
        }

        // GET: IssuedItem/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemMasterId = new SelectList(db.ItemMasters, "Id", "Name", issuedItem.ItemMasterId);
            return View(issuedItem);
        }

        // POST: IssuedItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemMasterId,IssuedOn,Quantity,IssuedBy,IssuedTo,OrgId,ItemMaster")] IssuedItem issuedItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(issuedItem).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a IssuedItem record");
                return RedirectToAction("Index");
            }
            ViewBag.ItemMasterId = new SelectList(db.ItemMasters, "Id", "Name", issuedItem.ItemMasterId);
            DisplayErrorMessage();
            return View(issuedItem);
        }

        // GET: IssuedItem/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            if (issuedItem == null)
            {
                return HttpNotFound();
            }
            return View(issuedItem);
        }

        // POST: IssuedItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            IssuedItem issuedItem = db.IssuedItems.Find(id);
            db.IssuedItems.Remove(issuedItem);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a IssuedItem record");
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
