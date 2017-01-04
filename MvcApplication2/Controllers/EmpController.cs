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
using System.Web.Security;

namespace MvcApplication2.Controllers
{
    [Authorize]
    public class EmpController : BaseController
    {
        private PEntities db = new PEntities();

        // GET: /Emp/
        public ActionResult Index(int? page)
        {
            var employees = db.Employees.Include(e => e.Organization);
            foreach (var em in employees)
            {
                em.EmpType = employees.Any(x => x.gid == em.Manager) ? employees.FirstOrDefault(x => x.gid == em.Manager).FirstName : em.Manager.HasValue?em.Manager.ToString():"-";
                em.EmpType += "#" + (db.OrgLocations.Any(x => x.Id == em.OrgLocationId && x.OrgId == em.OrgId) ? db.OrgLocations.FirstOrDefault(x => x.Id == em.OrgLocationId && x.OrgId == em.OrgId).LocationName : "");
                em.EmpType += "#" + (employees.Any(x => x.gid == em.Reviewer) ? employees.FirstOrDefault(x => x.gid == em.Reviewer).FirstName : em.Reviewer.HasValue ? em.Reviewer.ToString() : "-");
            }
            //return View(employees.ToList());
            return View(employees.OrderBy( o => o.gid).ToPagedList((page ?? 1), Paging.PageSize));
        }

        // GET: /Emp/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /Emp/Create
        public ActionResult Create()
        {
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        // POST: /Emp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="gid,FirstName,LastName,Department,Manager,Email,Phone,OrgId,Active,UserId,Designation,DoJ,ProfilePix,OrgEmpId,LocationId,Mobile,FirstTime,DoB,Gender,EmpType,LastVisit,Reviewer")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.gid = Guid.NewGuid();
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", employee.OrgId);
            return View(employee);
        }

        // GET: /Emp/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            string username = "sanjuan\\" + employee.Email.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
            ViewBag.OrgLocationId = new SelectList(db.OrgLocations.Where(x=>x.OrgId==1).ToList(), "Id", "LocationName", employee.OrgLocationId);
            ViewBag.manager = new SelectList(db.Employees.Where(x => x.Active).Select(y => new {gid=y.gid,Name=y.FirstName+" "+y.LastName }).ToList(), "gid", "Name", employee.Manager);
            var potentialManagersList = db.Employees.Select(x => new { Name = x.FirstName + " " + x.LastName, gid = x.gid });
            ViewBag.ReviewerList = new SelectList(potentialManagersList, "gid", "Name", employee.Reviewer);
            ViewBag.UserRoles = new SelectList(Roles.GetAllRoles().Where(x=>x.ToLower()!="admin").ToArray(), Roles.GetRolesForUser(username).FirstOrDefault());
            return View(employee);
        }

        // POST: /Emp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "gid,FirstName,OrgLocationId,LastName,Department,Manager,Email,Phone,OrgId,Active,UserId,Designation,DoJ,OrgEmpId,LocationId,Mobile,FirstTime,DoB,Gender,EmpType,LastVisit,Reviewer")] Employee employee, string UserRoles)
        {
            if (ModelState.IsValid)
            {
                var dbemp = db.Employees.FirstOrDefault(x => x.gid == employee.gid);
                dbemp.FirstName = propertyStrCompare(dbemp.FirstName,employee.FirstName);
                dbemp.LastName = propertyStrCompare(dbemp.LastName, employee.LastName);
                dbemp.Department = propertyStrCompare(dbemp.Department, employee.Department);
                dbemp.Designation = propertyStrCompare(dbemp.Designation, employee.Designation);
                dbemp.Email = propertyStrCompare(dbemp.Email, employee.Email);
                dbemp.Gender = propertyStrCompare(dbemp.Gender, employee.Gender);
                dbemp.Mobile = propertyStrCompare(dbemp.Mobile, employee.Mobile);
                dbemp.OrgEmpId = propertyStrCompare(dbemp.OrgEmpId, employee.OrgEmpId);
                dbemp.Phone = propertyStrCompare(dbemp.Phone, employee.Phone);

                dbemp.Active = propertyCompare<bool>(dbemp.Active, employee.Active).Value;
                dbemp.DoB = propertyCompare<DateTime>(dbemp.DoB, employee.DoB);
                dbemp.DoJ = propertyCompare<DateTime>(dbemp.DoJ, employee.DoJ);
                dbemp.LocationId = propertyCompare<Guid>(dbemp.LocationId, employee.LocationId);
                dbemp.Manager = propertyCompare<Guid>(dbemp.Manager, employee.Manager);
                dbemp.OrgLocationId = propertyCompare<int>(dbemp.OrgLocationId, employee.OrgLocationId);
                dbemp.Reviewer = propertyCompare<Guid>(dbemp.Reviewer, employee.Reviewer);


                db.Entry(dbemp).State = EntityState.Modified;
                db.SaveChanges();

                string username = "sanjuan\\"+dbemp.Email.Split(new string[]{"@"},StringSplitOptions.RemoveEmptyEntries)[0].ToLower();

                Roles.RemoveUserFromRoles(username, Roles.GetRolesForUser(username));
                Roles.AddUserToRole(username, UserRoles);
                return RedirectToAction("Index");
            }
            ViewBag.OrgLocationId = new SelectList(db.OrgLocations.Where(x => x.OrgId == 1).ToList(), "Id", "LocationName", employee.OrgLocationId);
            return View(employee);
        }
        private string propertyStrCompare(string dbvalue, string newvalue)
        {
            return !string.IsNullOrEmpty(dbvalue) && dbvalue != newvalue ? newvalue : dbvalue;
        }

        private Nullable<T> propertyCompare<T>(Nullable<T> dbvalue, Nullable<T> newvalue) where T:struct
        {
            return newvalue.HasValue && !Equals(dbvalue,newvalue) ? newvalue : dbvalue;
        }


        // GET: /Emp/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: /Emp/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
