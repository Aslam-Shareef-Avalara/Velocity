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
    public class VendorContactController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: VendorContact/Index
        public ActionResult Index()
        {
            var vendorContact = db.VendorContacts.Include(v => v.Vendor);
            return View(vendorContact.ToList());
        }

        /*
        // GET: VendorContact/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorContact vendorContact = db.VendorContacts.Find(id);
            if (vendorContact == null)
            {
                return HttpNotFound();
            }
            return View(vendorContact);
        }
        */

        // GET: VendorContact/Create
        public ActionResult Create()
        {
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            return View();
        }

        // POST: VendorContact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VendorId,FirstName,LastName,Phone,Mobile,email,Vendor")] VendorContact vendorContact)
        {
            if (ModelState.IsValid)
            {
                db.VendorContacts.Add(vendorContact);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a VendorContact record");
                return RedirectToAction("Index");
            }

            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorContact.VendorId);
            DisplayErrorMessage();
            return View(vendorContact);
        }

        // GET: VendorContact/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorContact vendorContact = db.VendorContacts.Find(id);
            if (vendorContact == null)
            {
                return HttpNotFound();
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorContact.VendorId);
            return View(vendorContact);
        }

        // POST: VendorContact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VendorId,FirstName,LastName,Phone,Mobile,email,Vendor")] VendorContact vendorContact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendorContact).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a VendorContact record");
                return RedirectToAction("Index");
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorContact.VendorId);
            DisplayErrorMessage();
            return View(vendorContact);
        }

        // GET: VendorContact/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorContact vendorContact = db.VendorContacts.Find(id);
            if (vendorContact == null)
            {
                return HttpNotFound();
            }
            return View(vendorContact);
        }

        // POST: VendorContact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VendorContact vendorContact = db.VendorContacts.Find(id);
            db.VendorContacts.Remove(vendorContact);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a VendorContact record");
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
