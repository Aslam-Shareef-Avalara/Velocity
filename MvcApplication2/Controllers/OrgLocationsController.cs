using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using X.Scaffolding.Core;
using PagedList;
using DataService;

namespace MvcApplication2.Controllers
{
    [Authorize]
    public class OrgLocationsController : BaseController
    {
        

        // GET: /OrgLocations/
        public ActionResult Index(int? page)
        {
            
            return View(db.OrgLocations.OrderBy( o => o.Id).ToPagedList((page ?? 1), Paging.PageSize));
        }

        // GET: /OrgLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrgLocation orgLocation = db.OrgLocations.Find(id);
            if (orgLocation == null)
            {
                return HttpNotFound();
            }
            return View(orgLocation);
        }

        // GET: /OrgLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /OrgLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,LocationName,OrgId,Address1,Address2,City,State,Country,Zip")] OrgLocation orgLocation)
        {
            if (ModelState.IsValid)
            {
                db.OrgLocations.Add(orgLocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orgLocation);
        }

        // GET: /OrgLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrgLocation orgLocation = db.OrgLocations.Find(id);
            if (orgLocation == null)
            {
                return HttpNotFound();
            }
            return View(orgLocation);
        }

        // POST: /OrgLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,LocationName,OrgId,Address1,Address2,City,State,Country,Zip")] OrgLocation orgLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orgLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orgLocation);
        }

        // GET: /OrgLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrgLocation orgLocation = db.OrgLocations.Find(id);
            if (orgLocation == null)
            {
                return HttpNotFound();
            }
            return View(orgLocation);
        }

        // POST: /OrgLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrgLocation orgLocation = db.OrgLocations.Find(id);
            db.OrgLocations.Remove(orgLocation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
