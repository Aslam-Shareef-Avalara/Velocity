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
    public class AssetController : BaseController
    {
        private InvMgmtEntities invDB = new InvMgmtEntities();

        // GET: Asset/Index
        public ActionResult Index()
        {
            var asset = invDB.Assets.Include(a => a.ItemMaster);
            return View(asset.ToList());
        }

        /*
        // GET: Asset/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }
        */

        // GET: Asset/Create
        public ActionResult Create()
        {
            ViewBag.ItemMasterId = new SelectList(invDB.ItemMasters, "Id", "Name");
            return View();
        }

        // POST: Asset/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemMasterId,GRNId,IsAssetized,AssetTag,AssetizedBy,AssetizedDate,Comment,IsIssued,ItemMaster,AssetIssues")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                invDB.Assets.Add(asset);
                invDB.SaveChanges();
                DisplaySuccessMessage("Has append a Asset record");
                return RedirectToAction("Index");
            }

            ViewBag.ItemMasterId = new SelectList(invDB.ItemMasters, "Id", "Name", asset.ItemMasterId);
            DisplayErrorMessage();
            return View(asset);
        }

        // GET: Asset/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = invDB.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemMasterId = new SelectList(invDB.ItemMasters, "Id", "Name", asset.ItemMasterId);
            return View(asset);
        }

        // POST: Asset/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemMasterId,GRNId,IsAssetized,AssetTag,AssetizedBy,AssetizedDate,Comment,IsIssued,ItemMaster,AssetIssues")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                invDB.Entry(asset).State = EntityState.Modified;
                invDB.SaveChanges();
                DisplaySuccessMessage("Has update a Asset record");
                return RedirectToAction("Index");
            }
            ViewBag.ItemMasterId = new SelectList(invDB.ItemMasters, "Id", "Name", asset.ItemMasterId);
            DisplayErrorMessage();
            return View(asset);
        }

        // GET: Asset/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = invDB.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        // POST: Asset/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Asset asset = invDB.Assets.Find(id);
            invDB.Assets.Remove(asset);
            invDB.SaveChanges();
            DisplaySuccessMessage("Has delete a Asset record");
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
                invDB.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
