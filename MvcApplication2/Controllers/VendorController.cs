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
    public class VendorController : BaseController
    {
        private InvMgmtEntities db1 = new InvMgmtEntities();

        // GET: Vendor/Index
        public ActionResult Index()
        {
            var vendor = db1.Vendors.Include(v => v.LegalEntity);
            return View(vendor.ToList());
        }

        /*
        // GET: Vendor/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }
        */

        // GET: Vendor/Create
        public ActionResult Create()
        {
            ViewBag.LegalEntityId = new SelectList(db1.LegalEntities, "Id", "Name");
            return View();
        }

        // POST: Vendor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,LegalEntityId,email,Description,Date,OrgId,LegalEntity,VendorAddresses,VendorBankDetails,VendorContacts,VendorTaxDetails")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                db1.Vendors.Add(vendor);
                db1.SaveChanges();
                DisplaySuccessMessage("Has append a Vendor record");
                return RedirectToAction("Index");
            }

            ViewBag.LegalEntityId = new SelectList(db1.LegalEntities, "Id", "Name", vendor.LegalEntityId);
            DisplayErrorMessage();
            return View(vendor);
        }

        // GET: Vendor/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db1.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            ViewBag.LegalEntityId = new SelectList(db1.LegalEntities, "Id", "Name", vendor.LegalEntityId);
            return View(vendor);
        }

        // POST: Vendor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,LegalEntityId,email,Description,Date,OrgId,LegalEntity,VendorAddresses,VendorBankDetails,VendorContacts,VendorTaxDetails")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                db1.Entry(vendor).State = EntityState.Modified;
                db1.SaveChanges();
                DisplaySuccessMessage("Has update a Vendor record");
                return RedirectToAction("Index");
            }
            ViewBag.LegalEntityId = new SelectList(db1.LegalEntities, "Id", "Name", vendor.LegalEntityId);
            DisplayErrorMessage();
            return View(vendor);
        }

        // GET: Vendor/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db1.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // POST: Vendor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Vendor vendor = db1.Vendors.Find(id);
            db1.Vendors.Remove(vendor);
            db1.SaveChanges();
            DisplaySuccessMessage("Has delete a Vendor record");
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
