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
    public class VendorBankDetailController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: VendorBankDetail/Index
        public ActionResult Index()
        {
            var vendorBankDetail = db.VendorBankDetails.Include(v => v.Vendor);
            return View(vendorBankDetail.ToList());
        }

        /*
        // GET: VendorBankDetail/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorBankDetail vendorBankDetail = db.VendorBankDetails.Find(id);
            if (vendorBankDetail == null)
            {
                return HttpNotFound();
            }
            return View(vendorBankDetail);
        }
        */

        // GET: VendorBankDetail/Create
        public ActionResult Create()
        {
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            return View();
        }

        // POST: VendorBankDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VendorId,BeneficiaryName,AccountNumber,BankName,Branch,MICR,IFSC,Vendor")] VendorBankDetail vendorBankDetail)
        {
            if (ModelState.IsValid)
            {
                db.VendorBankDetails.Add(vendorBankDetail);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a VendorBankDetail record");
                return RedirectToAction("Index");
            }

            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorBankDetail.VendorId);
            DisplayErrorMessage();
            return View(vendorBankDetail);
        }

        // GET: VendorBankDetail/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorBankDetail vendorBankDetail = db.VendorBankDetails.Find(id);
            if (vendorBankDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorBankDetail.VendorId);
            return View(vendorBankDetail);
        }

        // POST: VendorBankDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VendorId,BeneficiaryName,AccountNumber,BankName,Branch,MICR,IFSC,Vendor")] VendorBankDetail vendorBankDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendorBankDetail).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a VendorBankDetail record");
                return RedirectToAction("Index");
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorBankDetail.VendorId);
            DisplayErrorMessage();
            return View(vendorBankDetail);
        }

        // GET: VendorBankDetail/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorBankDetail vendorBankDetail = db.VendorBankDetails.Find(id);
            if (vendorBankDetail == null)
            {
                return HttpNotFound();
            }
            return View(vendorBankDetail);
        }

        // POST: VendorBankDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VendorBankDetail vendorBankDetail = db.VendorBankDetails.Find(id);
            db.VendorBankDetails.Remove(vendorBankDetail);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a VendorBankDetail record");
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
