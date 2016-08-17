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
    public class InventoryClassController : BaseController
    {
        private InvMgmtEntities db = new InvMgmtEntities();

        // GET: InventoryClass/Index
        public ActionResult Index()
        {
            return View(db.InventoryClasses.ToList());
        }

        /*
        // GET: InventoryClass/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryClass inventoryClass = db.InventoryClasses.Find(id);
            if (inventoryClass == null)
            {
                return HttpNotFound();
            }
            return View(inventoryClass);
        }
        */

        // GET: InventoryClass/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InventoryClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,OrgId,InventoryClassDepartmentMaps,ItemTypes")] InventoryClass inventoryClass)
        {
            if (ModelState.IsValid)
            {
                db.InventoryClasses.Add(inventoryClass);
                db.SaveChanges();
                DisplaySuccessMessage("Has append a InventoryClass record");
                return RedirectToAction("Index");
            }

            DisplayErrorMessage();
            return View(inventoryClass);
        }

        // GET: InventoryClass/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryClass inventoryClass = db.InventoryClasses.Find(id);
            if (inventoryClass == null)
            {
                return HttpNotFound();
            }
            return View(inventoryClass);
        }

        // POST: InventoryClass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,OrgId,InventoryClassDepartmentMaps,ItemTypes")] InventoryClass inventoryClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventoryClass).State = EntityState.Modified;
                db.SaveChanges();
                DisplaySuccessMessage("Has update a InventoryClass record");
                return RedirectToAction("Index");
            }
            DisplayErrorMessage();
            return View(inventoryClass);
        }

        // GET: InventoryClass/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryClass inventoryClass = db.InventoryClasses.Find(id);
            if (inventoryClass == null)
            {
                return HttpNotFound();
            }
            return View(inventoryClass);
        }

        // POST: InventoryClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            InventoryClass inventoryClass = db.InventoryClasses.Find(id);
            db.InventoryClasses.Remove(inventoryClass);
            db.SaveChanges();
            DisplaySuccessMessage("Has delete a InventoryClass record");
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
