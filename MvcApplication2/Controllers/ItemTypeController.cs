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
    public class ItemTypeController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: ItemType/Index
        public ActionResult Index()
        {
            var itemType = db.ItemTypes.Include(i => i.InventoryClass);
            return View(itemType.ToList());
        }

        /*
        // GET: ItemType/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }
        */

        // GET: ItemType/Create
        public ActionResult Create()
        {
            ViewBag.InventoryClassId = new SelectList(db.InventoryClasses, "Id", "Name");
            return View();
        }

        // POST: ItemType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,OrgId,InventoryClassId,InventoryClass,ItemMasters")] ItemType itemType)
        {
            if (ModelState.IsValid)
            {
                db.ItemTypes.Add(itemType);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a ItemType record");
                return RedirectToAction("Index");
            }

            ViewBag.InventoryClassId = new SelectList(db.InventoryClasses, "Id", "Name", itemType.InventoryClassId);
            DisplayErrorMessage();
            return View(itemType);
        }

        // GET: ItemType/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            ViewBag.InventoryClassId = new SelectList(db.InventoryClasses, "Id", "Name", itemType.InventoryClassId);
            return View(itemType);
        }

        // POST: ItemType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,OrgId,InventoryClassId,InventoryClass,ItemMasters")] ItemType itemType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemType).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a ItemType record");
                return RedirectToAction("Index");
            }
            ViewBag.InventoryClassId = new SelectList(db.InventoryClasses, "Id", "Name", itemType.InventoryClassId);
            DisplayErrorMessage();
            return View(itemType);
        }

        // GET: ItemType/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemType itemType = db.ItemTypes.Find(id);
            if (itemType == null)
            {
                return HttpNotFound();
            }
            return View(itemType);
        }

        // POST: ItemType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ItemType itemType = db.ItemTypes.Find(id);
            db.ItemTypes.Remove(itemType);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a ItemType record");
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
