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
    public class VendorTaxDetailController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: VendorTaxDetail/Index
        public ActionResult Index()
        {
            var vendorTaxDetail = db.VendorTaxDetails.Include(v => v.Vendor);
            return View(vendorTaxDetail.ToList());
        }

        /*
        // GET: VendorTaxDetail/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorTaxDetail vendorTaxDetail = db.VendorTaxDetails.Find(id);
            if (vendorTaxDetail == null)
            {
                return HttpNotFound();
            }
            return View(vendorTaxDetail);
        }
        */

        // GET: VendorTaxDetail/Create
        public ActionResult Create()
        {
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            return View();
        }

        // POST: VendorTaxDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VendorId,VATNumber,TINNumber,ServiceTaxNumber,PanNumber,Vendor")] VendorTaxDetail vendorTaxDetail)
        {
            if (ModelState.IsValid)
            {
                db.VendorTaxDetails.Add(vendorTaxDetail);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a VendorTaxDetail record");
                return RedirectToAction("Index");
            }

            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorTaxDetail.VendorId);
            DisplayErrorMessage();
            return View(vendorTaxDetail);
        }

        // GET: VendorTaxDetail/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorTaxDetail vendorTaxDetail = db.VendorTaxDetails.Find(id);
            if (vendorTaxDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorTaxDetail.VendorId);
            return View(vendorTaxDetail);
        }

        // POST: VendorTaxDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VendorId,VATNumber,TINNumber,ServiceTaxNumber,PanNumber,Vendor")] VendorTaxDetail vendorTaxDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendorTaxDetail).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a VendorTaxDetail record");
                return RedirectToAction("Index");
            }
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorTaxDetail.VendorId);
            DisplayErrorMessage();
            return View(vendorTaxDetail);
        }

        // GET: VendorTaxDetail/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorTaxDetail vendorTaxDetail = db.VendorTaxDetails.Find(id);
            if (vendorTaxDetail == null)
            {
                return HttpNotFound();
            }
            return View(vendorTaxDetail);
        }

        // POST: VendorTaxDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VendorTaxDetail vendorTaxDetail = db.VendorTaxDetails.Find(id);
            db.VendorTaxDetails.Remove(vendorTaxDetail);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a VendorTaxDetail record");
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
