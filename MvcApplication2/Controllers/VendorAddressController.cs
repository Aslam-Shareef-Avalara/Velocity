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
    public class VendorAddressController : Controller
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: VendorAddress/Index
        public ActionResult Index()
        {
            var vendorAddress = db.VendorAddresses.Include(v => v.Country).Include(v => v.State).Include(v => v.Vendor);
            return View(vendorAddress.ToList());
        }

        /*
        // GET: VendorAddress/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorAddress vendorAddress = db.VendorAddresses.Find(id);
            if (vendorAddress == null)
            {
                return HttpNotFound();
            }
            return View(vendorAddress);
        }
        */

        // GET: VendorAddress/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name");
            ViewBag.StateId = new SelectList(db.States, "Id", "Name");
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name");
            return View();
        }

        // POST: VendorAddress/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VendorId,Line1,Line2,City,StateId,County,CountryId,Zip,Phone1,Phone2,Landmark,Country,State,Vendor")] VendorAddress vendorAddress)
        {
            if (ModelState.IsValid)
            {
                db.VendorAddresses.Add(vendorAddress);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a VendorAddress record");
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", vendorAddress.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", vendorAddress.StateId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorAddress.VendorId);
            DisplayErrorMessage();
            return View(vendorAddress);
        }

        // GET: VendorAddress/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorAddress vendorAddress = db.VendorAddresses.Find(id);
            if (vendorAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", vendorAddress.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", vendorAddress.StateId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorAddress.VendorId);
            return View(vendorAddress);
        }

        // POST: VendorAddress/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VendorId,Line1,Line2,City,StateId,County,CountryId,Zip,Phone1,Phone2,Landmark,Country,State,Vendor")] VendorAddress vendorAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendorAddress).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a VendorAddress record");
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", vendorAddress.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", vendorAddress.StateId);
            ViewBag.VendorId = new SelectList(db.Vendors, "Id", "Name", vendorAddress.VendorId);
            DisplayErrorMessage();
            return View(vendorAddress);
        }

        // GET: VendorAddress/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorAddress vendorAddress = db.VendorAddresses.Find(id);
            if (vendorAddress == null)
            {
                return HttpNotFound();
            }
            return View(vendorAddress);
        }

        // POST: VendorAddress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            VendorAddress vendorAddress = db.VendorAddresses.Find(id);
            db.VendorAddresses.Remove(vendorAddress);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a VendorAddress record");
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
