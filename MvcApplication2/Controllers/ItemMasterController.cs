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
    public class ItemMasterController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: ItemMaster/Index
        public ActionResult Index()
        {
            var itemMaster = db.ItemMasters.Include(i => i.ItemType);
            return View(itemMaster.ToList());
        }

        /*
        // GET: ItemMaster/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemMaster itemMaster = db.ItemMasters.Find(id);
            if (itemMaster == null)
            {
                return HttpNotFound();
            }
            return View(itemMaster);
        }
        */

        // GET: ItemMaster/Create
        public ActionResult Create()
        {
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x=>x.OrgId==1), "Id", "Name");
            return View();
        }

        // POST: ItemMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ItemTypeId,Quantity,Perishable,OrgId,Assets,IssuedItems,ItemType")] ItemMaster itemMaster)
        {
            if (ModelState.IsValid)
            {
                db.ItemMasters.Add(itemMaster);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a ItemMaster record");
                return RedirectToAction("Index");
            }

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "Id", "Name", itemMaster.ItemTypeId);
            DisplayErrorMessage();
            return View(itemMaster);
        }

        // GET: ItemMaster/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemMaster itemMaster = db.ItemMasters.Find(id);
            if (itemMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "Id", "Name", itemMaster.ItemTypeId);
            return View(itemMaster);
        }

        // POST: ItemMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ItemTypeId,Quantity,Perishable,OrgId,Assets,IssuedItems,ItemType")] ItemMaster itemMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemMaster).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a ItemMaster record");
                return RedirectToAction("Index");
            }
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes, "Id", "Name", itemMaster.ItemTypeId);
            DisplayErrorMessage();
            return View(itemMaster);
        }

        // GET: ItemMaster/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemMaster itemMaster = db.ItemMasters.Find(id);
            if (itemMaster == null)
            {
                return HttpNotFound();
            }
            return View(itemMaster);
        }

        // POST: ItemMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ItemMaster itemMaster = db.ItemMasters.Find(id);
            db.ItemMasters.Remove(itemMaster);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a ItemMaster record");
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
