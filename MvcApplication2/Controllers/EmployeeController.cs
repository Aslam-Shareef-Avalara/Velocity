using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using X.Scaffolding.Core;
using PagedList;
using DataService;
using MvcApplication2.Models;
using System.Drawing;
using System.Collections;
using System.Web.Security;
using System.IO;

namespace MvcApplication2.Controllers
{
    [AuthLog(Roles = "Admin,User, Hr,Employee, HrAdmin")]
    public class EmployeeController : BaseController
    {
        private PEntities db = new PEntities();

        public string GetInlineImage(Guid g)
        {
            var e =  db.Employees.FirstOrDefault(x=>x.gid==g);
            if (e != null)
                return Convert.ToBase64String(e.ProfilePix);
            else
                return "";
        }

        public string Image(Guid g)
        {
            string basepath = Server.MapPath("~/attachments");
            if (System.IO.File.Exists(basepath+"\\" + g.ToString() + "profile.png"))
            {
                return Url.Content(basepath + "\\" + g.ToString() + "profile.png");
            }
            var e = db.Employees.FirstOrDefault(x => x.gid == g);
            if (e != null)
            {
                MemoryStream ms = new MemoryStream(e.ProfilePix);
                Bitmap bmp = new Bitmap(ms);
                bmp.Save(basepath + "\\" + g.ToString() + "profile.png");
                return Url.Content("~/attachments/" + g.ToString() + "profile.png");
            }
            else
                return "";
        }
        public ActionResult Reviewees()
        {
            EmployeeService empService = new EmployeeService(OrgId, AppName,currentUser);
            var reviewies = empService.GetReviewies(currentUser.gid);
            if (reviewies == null)
                return View("Message", (object)"You do not have any reviewees");
            return View(reviewies);
        }
        public ActionResult AboutMe()
        {
            EmployeeService empService = new EmployeeService(OrgId, AppName,currentUser);
            Hashtable reporteeList = new Hashtable();
            MvcApplication2.ViewModel.AboutMe me = new ViewModel.AboutMe();
            ActiveDirectoryHelper adHelper = new ActiveDirectoryHelper();
            me.Myself = currentUser;
            try
            {
                me.Coworkers = empService.GetCoworkers(currentUser.gid);
            }
            catch { me.Coworkers = new List<Employee>(); }
            string hradmin = "";
            //if (Roles.GetUsersInRole("HrAdmin") != null && Roles.GetUsersInRole("HrAdmin").Count() > 0)
            //    hradmin= Roles.GetUsersInRole("HrAdmin").FirstOrDefault();
            //else
            me.HrPerson = adHelper.GetHRManager() ;
            if (currentUser.Manager.HasValue)
            {
                me.Manager = empService.GetManager(currentUser.Manager.Value);// adHelper.GetMyManager();
            }
            else me.Manager = null;
            me.Org = db.Organizations.Where(x => x.Id == currentUser.OrgId).FirstOrDefault();
            
            var reportees = empService.GetReporteesOf(currentUser.gid);
            foreach (var reportee in reportees)
            {
                reporteeList[reportee.gid] = reportee.FirstName + " " + reportee.LastName;
            }
            me.Reportees = reporteeList;

           
            return View("Me",me);
        }

        public ActionResult PerformanceData(Guid reporteeID)
        {
            if (reporteeID == currentUser.gid)
                ViewBag.Salutation = "Your";
            else
                ViewBag.Salutation = db.Employees.FirstOrDefault(x => x.gid == reporteeID).FirstName + "'s";
            if (db.EvaluationRatings.Join(db.EvaluationCycles, e => e.EvalCycleId, g => g.Id, (e, g) => new { e, g }).Any(x => x.e.EmpId == reporteeID && x.g.EvaluationEnd < DateTime.Today && x.e.ManagerOverllRating.HasValue))
                ViewBag.PerformanceData = db.EvaluationRatings.Join(db.EvaluationCycles, e => e.EvalCycleId, g => g.Id, (e, g) => new { e, g }).Where(x => x.e.EmpId == reporteeID && x.g.EvaluationEnd < DateTime.Today).OrderBy(x => x.g.EvaluationEnd).Select(x => new PerformanceChartModel { Rating = x.e.ManagerOverllRating.Value, PETitle = x.g.Title }).ToList();
            else ViewBag.PerformanceData = new List<PerformanceChartModel>();
            try
            {
                if (ViewBag.PerformanceData.Count > 0)
                {
                    var SuccessPerformanceData = db.ManagerEvaluations
                        .Join(db.Goals, _me => _me.GoalId, g => g.gid, (_me, g) => new { _me, g })
                        .Join(db.EvaluationCycles, mg => mg._me.EvalCycleId, ec => ec.Id, (mg, ec) => new { mg, ec })
                        .Where(x => x.mg.g.OrgId == OrgId && x.mg.g.Fixed == true && x.mg._me.EmployeeId == reporteeID && x.ec.EvaluationEnd < DateTime.Today)
                        .OrderBy(x => x.mg.g.Title).ThenBy(x => x.ec.EvaluationEnd)
                        .Select(x => new PerformanceChartModel
                        {
                            Rating = x.mg._me.GoalRating.HasValue ? (decimal)x.mg._me.GoalRating : 0,
                            PETitle = x.ec.Title,
                            TextData1 = x.mg.g.Title,
                            Date1 = x.ec.EvaluationEnd.Value
                        });
                    var listOfSuccessTraits = SuccessPerformanceData.Select(x => x.TextData1).Distinct();
                    Hashtable ht = new Hashtable();
                    foreach (string trait in listOfSuccessTraits)
                    {
                        ht[trait] = SuccessPerformanceData.Where(x => x.TextData1 == trait).OrderBy(x => x.Date1).ToList();
                    }
                    ViewBag.SuccessTraitsData = ht;
                }
            }
            catch (Exception x) { logger.Error("When getting success trait data" + x.Message + x.StackTrace, x); }
            return View("PerformanceGraphs");
        }
        public JsonResult currentcyclestatus()
        {
            EvaluationCycle eCycle = CurrentEvalCycle;
            EvaluationCycle gCycle = CurrentGoalCycle;
            List<dynamic> v = new List<dynamic>();
            string formattedDayDateTime = "({0} days and {1} hr : {2} mins remaining)";
            string formattedDateTime = "({0} hr : {1} mins remaining)";
            if (gCycle != null && gCycle.GoalStartDate <= DateTime.Today && gCycle.GoalEndDate >= DateTime.Today)
            {

                TimeSpan t = (TimeSpan)(gCycle.GoalEndDate - DateTime.Now);
                v.Add(new
                {
                    title = gCycle.Title,
                    description = gCycle.Description,
                    start = gCycle.GoalStartDate,
                    end = gCycle.GoalEndDate,
                    cycletype = gCycle.Title + " - Goal Setting",
                    daysRemaining = (t.Days > 0) ? string.Format(formattedDayDateTime, t.Days, t.Hours, t.Minutes) : string.Format(formattedDateTime, t.Hours, t.Minutes),
                    percentComplete = (int)(100.0F * ((float)((TimeSpan)(DateTime.Today - gCycle.GoalStartDate)).Days / (float)((TimeSpan)(gCycle.GoalEndDate - gCycle.GoalStartDate)).Days))
                });

            }
            if (eCycle != null && eCycle.EvaluationStart <= DateTime.Today && eCycle.EvaluationEnd >= DateTime.Today)
            {
                TimeSpan t = (TimeSpan)(eCycle.EvaluationEnd - DateTime.Now);
                v.Add(new
                {
                    title = eCycle.Title,
                    description = eCycle.Description,
                    start = eCycle.EvaluationStart,
                    end = eCycle.EvaluationEnd,
                    cycletype = eCycle.Title + " - Evaluation Process",
                    daysRemaining = (t.Days > 0) ? string.Format(formattedDayDateTime, t.Days, t.Hours, t.Minutes) : string.Format(formattedDateTime, t.Hours, t.Minutes),
                    percentComplete = (int)(100.0F * ((float)((TimeSpan)(DateTime.Today - eCycle.EvaluationStart)).Days / (float)((TimeSpan)(eCycle.EvaluationEnd - eCycle.EvaluationStart)).Days))
                });

            }
            if (v.Count > 0)
            {
                return Json(v, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                title = "",
                description = ""
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCurrentCycleStatuses(string reporteeid)
        {
            string employeeid = reporteeid;
            if (string.IsNullOrEmpty(employeeid))
            {
                employeeid = currentUser.gid.ToString();
            }
            List<EvaluationCycleExtended> cyclestatuses = new List<EvaluationCycleExtended>();
            Hashtable ht = new EmployeeService(OrgId, AppName as string, currentUser).GetPECycleStatus(Guid.Parse(employeeid));
            if (ht.ContainsKey(CONSTANTS.GOAL_SETTING_CYCLE))
            {
                
                cyclestatuses.Add((EvaluationCycleExtended)ht[CONSTANTS.GOAL_SETTING_CYCLE]);
            }
            if (ht.ContainsKey(CONSTANTS.EVALUATION_CYCLE))
            {
                 cyclestatuses.Add((EvaluationCycleExtended)ht[CONSTANTS.EVALUATION_CYCLE]);
            }
            return Json(cyclestatuses,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineEmployeeReport()
        {
            int[] emps = new ActiveDirectoryHelper().OnlineEmployees();
            return Json(new { total = emps[0], online = emps[1] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MarkBadgeAsRead(long? id)
        {
            try
            {
                Badges badgeSvc = new Badges(OrgId, AppName,currentUser);
                badgeSvc.DeleteBadges(new List<long> { id.Value });
                return Json(new { deleted = 1, idofbadge = id },JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new { deleted = 0, idofbadge = id }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getmyteambadges()
        {
            EmployeeService empService = new EmployeeService(OrgId,AppName,currentUser);
            var reportees = empService.GetReportees(currentUser.gid);
            Badges badgeSvc = new Badges(OrgId, AppName,currentUser);
            List<dynamic> badges = new List<dynamic>();
            if (reportees != null && reportees.Count > 0)
            {

                foreach (Employee e in reportees)
                {
                    var b = badgeSvc.GetBadgesFrom(e.gid, currentUser.gid);

                    if (b != null && b.Count > 0)
                    {
                        //if (b.Count > 1)
                        //    badges.Add(new { eid = e.gid, badgecount = b.Count, tooltip = "" });
                        //else
                            badges.Add(new { eid = e.gid, badgecount = b.Count, tooltip = b.First().Description });
                    }
                }
                if (badges != null && badges.Count > 0)
                {
                    return Json(badges, JsonRequestBehavior.AllowGet);
                }


            }
            return Json(badges, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getallbadges()
        {

            Badges badgeSvc = new Badges(OrgId, AppName,currentUser);
            var badges = badgeSvc.GetAllBadgesFor(currentUser.gid);
            if (badges != null && badges.Count > 0)
            {
                if (string.IsNullOrEmpty(ctx.Request.QueryString["full"]))
                    return Json((object)badges.Count, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(badges, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ctx.Request.QueryString["full"]))
                    return Json((object)0, JsonRequestBehavior.AllowGet);
                else

                    return Json(new List<Badge>(), JsonRequestBehavior.AllowGet);

            }
                
        }
        // GET: /Employee/
        public ActionResult Index(string gid)
        {
            string username = null;
            Hashtable reporteeList = new Hashtable();
            #region CommentedCodeForFetchingProfileImage
            //try
            //{
            //    // Create the web request with the REST URL.
            //    HttpWebRequest request =
            //       WebRequest.Create("https://outlook.office365.com/owa/service.svc/s/GetPersonaPhoto?email=aslam.shareef%40avalara.com&UA=0&size=HR96x96")
            //       as HttpWebRequest;
            //    // Submit the request.
            //    using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
            //    {
            //        // Take the response and save it as an image.
            //        Bitmap image = new Bitmap(resp.GetResponseStream());
            //        image.Save("aslam.jpg");
            //    }
            //}
            //catch (Exception xc)
            //{ }
            //   var employees = db.Employees.Include(e => e.Employee1).Include(e => e.Organization).Include(e => e.Role1);
            //return View(await employees.ToListAsync());     
            #endregion
            Guid g = new Guid();
            if (string.IsNullOrEmpty(gid))
                g = currentUser.gid;
            else
                g = Guid.Parse(gid);
           
                var e =db.Employees.FirstOrDefault(x => x.gid == g );
                //if (e != null && e.UserId!=null)
                //{
                //    var ae = db.aspnet_Users.FirstOrDefault(x => x.UserId == e.UserId);
                //    if (ae != null)
                //    {
                //        username = ae.LoweredUserName;
                //    }
                //}
                EmployeeService empService = new EmployeeService(OrgId, AppName,currentUser);
                var reportees = empService.GetReportees(g);
                foreach (var reportee in reportees)
                {
                    reporteeList[reportee.gid] = reportee;
                }
            
            
            ViewBag.Me = currentUser;
            ViewBag.reporteeid = gid;
            ViewBag.CurrentReportee = e;
            return View("MyTeam", reporteeList);
        }

        // GET: /Employee/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /Employee/Create
        public ActionResult Create()
        {
            ViewBag.Manager = new SelectList(db.Employees, "gid", "FirstName");
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name");
            ViewBag.Role = new SelectList(db.Roles, "Id", "Role1");
            return View();
        }

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "gid,FirstName,LastName,Department,Manager,Email,Phone,OrgId,Active,Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.gid = Guid.NewGuid();
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Manager = new SelectList(db.Employees, "gid", "FirstName", employee.Manager);
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", employee.OrgId);
           // ViewBag.Role = new SelectList(db.Roles, "Id", "Role1", employee.Role);
            return View(employee);
        }

        // GET: /Employee/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            
            if (id == null)
            {
                id = currentUser.gid;
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.HrEdit = User.IsInRole("Hr") || User.IsInRole("HrAdmin");
            EmployeeExtended emp = new EmployeeExtended(employee);
            var potentialManagersList = db.Employees.Select(x => new { Name = x.FirstName + " " + x.LastName, gid = x.gid });
            ViewBag.ManagerList = new SelectList(potentialManagersList, "gid", "Name", emp.Manager);
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", emp.OrgId);
            //ViewBag.Role = new SelectList(db.Roles, "Id", "Role1", employee.Role);
            return View(emp);
        }

        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        public async Task<ActionResult> Edit([Bind(Include = "gid,FirstName,LastName,Department,Manager,Email,Phone,OrgId,Active,Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return View(employee);
            }
            ViewBag.Manager = new SelectList(db.Employees, "gid", "FirstName", employee.Manager);
            ViewBag.OrgId = new SelectList(db.Organizations, "Id", "Name", employee.OrgId);
          //  ViewBag.Role = new SelectList(db.Roles, "Id", "Role1", employee.Role);
            return View(employee);
        }

        [HttpPost]
        public ActionResult EditForEmployee(Employee e)
        {
            bool updatecurrentuser = e.gid == currentUser.gid;
            

            Employee emp = db.Employees.FirstOrDefault(x=>x.gid==e.gid);
            if (!string.IsNullOrEmpty(e.FirstName))
            emp.FirstName = e.FirstName;
            if (!string.IsNullOrEmpty(e.LastName))
            emp.LastName = e.LastName;
            if (!string.IsNullOrEmpty(e.Phone))
            emp.Phone = e.Phone;
            if (!string.IsNullOrEmpty(e.Mobile))
            emp.Mobile = e.Mobile;
            db.SaveChanges();
            string querystring = "";
            if(updatecurrentuser)
                currentUser = emp;
            TempData["msg"] = "Saved successfully!";
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                querystring = "?id=" + Request.QueryString["id"];
            return Redirect("~/employee/edit" + querystring);
        }

        [HttpPost]
        public ActionResult EditForHr(Employee e)
        {
            Employee emp = db.Employees.FirstOrDefault(x => x.gid == e.gid);
            bool updatecurrentuser = e.gid == currentUser.gid;
            if (e.Manager.HasValue)
                emp.Manager = e.Manager;
            if (!string.IsNullOrEmpty(e.Department))
            emp.Department = e.Department;
            if (!string.IsNullOrEmpty(e.Designation))
                emp.Designation = e.Designation;
            if (!e.Reviewer.HasValue)
                emp.Reviewer= e.Reviewer;
            if(e.DoB.HasValue)
                emp.DoB = e.DoB;
            if (e.DoJ.HasValue)
                emp.DoJ = e.DoJ;
            if(!string.IsNullOrEmpty(Request.Form["Active"]))
                emp.Active = Request.Form["Active"].ToLower() == "on" || Request.Form["Active"].ToLower() == "checked" || Request.Form["Active"].ToLower() == "true";
            db.SaveChanges();
            string querystring = "";
            if(!string.IsNullOrEmpty(Request.QueryString["id"]))
                querystring = "?id=" + Request.QueryString["id"];
            TempData["msg"] = "Saved successfully!";
            if (updatecurrentuser)
                currentUser = emp;
            return Redirect("~/employee/edit" + querystring);
        }

        // GET: /Employee/Delete/5
        //public async Task<ActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = await db.Employees.FindAsync(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(employee);
        //}

        //// POST: /Employee/Delete/5
        //[HttpPost, ActionName("Delete")]
        ////[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(Guid id)
        //{
        //    Employee employee = await db.Employees.FindAsync(id);
        //    db.Employees.Remove(employee);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void savelocation(int locationid)
        {
            var e = db.Employees.FirstOrDefault(x => x.gid == currentUser.gid);
            e.OrgLocationId = locationid;
            var o = db.OrgLocations.FirstOrDefault(x => x.Id == locationid);
            e.OrgId = o.Id;
            db.SaveChanges();
        }

    }
}
