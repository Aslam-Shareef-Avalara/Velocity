﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public class EvaluationCycleExtended : EvaluationCycle
    {

        public GoalCycleStatus GoalSettingStatus = GoalCycleStatus.NO_ACTION_REQUIRED;
        public EvalCycleStatus EvaluationStatus = EvalCycleStatus.NO_ACTION_REQUIRED;

        public EvaluationCycleExtended(EvaluationCycle c)
        {

            this.Description = c.Description;
            this.EvaluationEnd = c.EvaluationEnd;
            this.EvaluationStart = c.EvaluationStart;
            this.GoalEndDate = c.GoalEndDate;
            this.GoalStartDate = c.GoalStartDate;
            this.Id = c.Id;
            this.OrgId = c.OrgId;
            this.Title = c.Title;

        }
    }

    public class EmployeeExtended : Employee
    {
        public bool HasReportees = false;
        public Guid RoleId = new Guid();
        public string Role = "";
        public Employee ManagerObj = new Employee() { FirstName = "Not Assigned" };
        public Employee ReviewerObj = new Employee() { FirstName = "Not Assigned" };
        public string Organization = "";
        public EmployeeExtended(Employee e)
        {
            this.Active = e.Active;
            this.Department = e.Department;
            this.Designation = e.Designation;
            this.DoB = e.DoB;
            this.DoJ = e.DoJ;
            this.Email = e.Email;
            this.EmpType = e.EmpType;
            this.FirstName = e.FirstName;
            this.FirstTime = e.FirstTime;
            this.Gender = e.Gender;
            this.gid = e.gid;
            this.LastName = e.LastName;
            this.LocationId = e.LocationId;
            this.Manager = e.Manager;
            this.Mobile = e.Mobile;
            this.OrgEmpId = e.OrgEmpId;
            this.OrgId = e.OrgId;
            this.Phone = e.Phone;
            this.OrgLocationId = e.OrgLocationId;
            this.ProfilePix = e.ProfilePix;
            this.UserId = e.UserId;
            this.LastVisit = e.LastVisit;
            this.Reviewer = e.Reviewer;
            using (PEntities dbx = new PEntities())
            {
                if (dbx.Employees.Any(x => x.gid == e.Manager && x.Active))
                {
                    ManagerObj = dbx.Employees.FirstOrDefault(x => x.gid == e.Manager && x.Active);
                }
                if (e.Reviewer != null && dbx.Employees.Any(x => x.gid == e.Reviewer && x.Active))
                {
                    ReviewerObj = dbx.Employees.FirstOrDefault(x => x.gid == e.Reviewer && x.Active);
                }
                Organization = dbx.Organizations.FirstOrDefault(x => x.Id == e.OrgId).Name;
            }
        }
    }

    public class EmployeeService : BaseModel, IEmployeeService
    {
        public EmployeeService(int orgid, string appname, Employee emp)
            : base(orgid, appname, emp)
        {

        }
        public void CreateEmployee(Employee employee, string username)
        {
            bool empExists = false;
            using (PEntities dbx = new PEntities())
            {

                if (empExists = dbx.Employees.Any(x => x.gid == employee.gid))
                {
                    employee = dbx.Employees.FirstOrDefault(x => x.gid == employee.gid);

                }
                else
                {

                    employee.OrgId = 1;// dbx.OrgLocations.Where(x=>x.OrgId==employee.OrgId)
                }
                aspnet_Users user = dbx.aspnet_Users.Where(x => x.LoweredUserName == username.ToLower()).SingleOrDefault();
                if (user == null) throw new Exception("user is not added to any role yet.");
                if (employee.Manager.HasValue)
                {
                    Guid mgrId = employee.Manager.Value;
                    Employee mgr = dbx.Employees.SingleOrDefault(x => x.gid == mgrId);
                    if (mgr == null)
                        employee.Manager = null;
                }

                employee.UserId = user.UserId;
                if (!empExists)
                    dbx.Employees.Add(employee);
                dbx.SaveChanges();
            }
        }

        public List<OnGoingFeedback> GetOnGoingFeedbackFor(Guid empId, long evalCycleId)
        {
            using (PEntities dbx = new PEntities())
            {
                var empR = dbx.EvaluationRatings.FirstOrDefault(e => e.EmpId == empId && e.EvalCycleId == evalCycleId);
                if (empR != null && !string.IsNullOrEmpty(empR.OnGoingFeedBack))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<OnGoingFeedback>>(empR.OnGoingFeedBack);
                }
                return null;

            }
        }

        public class OnGoingFeedback
        {
            
            public DateTime FeedbackDate;
            public string FeedbackText;
            public long evalCycleId;
            public Guid EmployeeId;
            public Guid ManagerId;
            public string ManagerName;
            public bool isPublic=true;
        }
        public void UpdateActiveFeedbackVisibility(Guid empId, long evalCycleId, bool ispublic, string fbtext)
        {
            using (PEntities dbx = new PEntities())
            {
                var currentemp = dbx.Employees.FirstOrDefault(x => x.gid == empId);
                var currentManager = dbx.Employees.FirstOrDefault(x => x.gid == currentemp.Manager);
                var empR = dbx.EvaluationRatings.FirstOrDefault(e => e.EmpId == empId && e.EvalCycleId == evalCycleId);

                if (empR != null)
                {

                    var feedbacks = new List<OnGoingFeedback>();
                    if (!string.IsNullOrEmpty(empR.OnGoingFeedBack))
                        feedbacks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OnGoingFeedback>>(empR.OnGoingFeedBack);

                    if (feedbacks.Any(x => x.EmployeeId == empId && x.evalCycleId == evalCycleId && x.FeedbackText == fbtext && x.isPublic!=ispublic))
                    {
                        var fb = feedbacks.FirstOrDefault(x => x.EmployeeId == empId && x.evalCycleId == evalCycleId && x.FeedbackText == fbtext && x.isPublic != ispublic);
                        feedbacks.Remove(fb);
                        fb.isPublic = ispublic;
                        feedbacks.Add(fb);
                    }
                    
                    string feedback = Newtonsoft.Json.JsonConvert.SerializeObject(feedbacks);
                    empR.OnGoingFeedBack = feedback;
                    dbx.SaveChanges();
                }
            }
        
        }
        public void SaveOnGoingFeedbackFor(Guid empId, long evalCycleId, string feedback, bool isprivate)
        {
            try
            {
                using (PEntities dbx = new PEntities())
                {
                    var currentemp = dbx.Employees.FirstOrDefault(x => x.gid == empId);
                    var currentManager = dbx.Employees.FirstOrDefault(x => x.gid == CurrentUser.gid);
                    var empR = dbx.EvaluationRatings.FirstOrDefault(e => e.EmpId == empId && e.EvalCycleId == evalCycleId);
                    if (empR == null)
                    {
                        var evalRating = new EvaluationRating();
                        evalRating.EvalCycleId = evalCycleId;
                        evalRating.EmpId = empId;
                        evalRating.ManagerId = currentemp.Manager;
                        //evalRating.SelfOverallRating = avgSelfRating;
                        //evalRating.ManagerOverllRating = avgMgrRating;
                        dbx.EvaluationRatings.Add(evalRating);
                        dbx.SaveChanges();
                        empR = dbx.EvaluationRatings.FirstOrDefault(e => e.EmpId == empId && e.EvalCycleId == evalCycleId);
                    }
                    if (empR != null)
                    {

                        var feedbacks = new List<OnGoingFeedback>();
                        if (!string.IsNullOrEmpty(empR.OnGoingFeedBack))
                            feedbacks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OnGoingFeedback>>(empR.OnGoingFeedBack);
                        feedbacks.Add(new OnGoingFeedback()
                        {
                            EmployeeId = empId,
                            evalCycleId = evalCycleId,
                            FeedbackDate = DateTime.Now,
                            FeedbackText = feedback,
                            ManagerId = currentManager.gid,
                            ManagerName = currentManager.FullName(),
                            isPublic = !isprivate
                        });
                        feedback = Newtonsoft.Json.JsonConvert.SerializeObject(feedbacks);
                        empR.OnGoingFeedBack = feedback;
                        dbx.SaveChanges();
                    }

                }
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }

        }
        public Employee GetEmployee(Guid empId)
        {
            using (PEntities dbx = new PEntities())
            {
                var emp = dbx.Employees.FirstOrDefault(e => e.gid == empId);
                return emp;
            }
        }


        public List<Employee> GetCoworkers(Guid empId)
        {
            List<Employee> co_workers = new List<Employee>();
            using (var dbx = new PEntities())
            {
                Employee self = dbx.Employees.FirstOrDefault(x => x.gid == empId);
                if (self != null)
                {
                    try
                    {
                        co_workers = dbx.Employees.Where(x => x.Manager == self.Manager && x.Active && x.OrgEmpId != self.OrgEmpId).ToList();
                    }
                    catch { }
                }
            }
            return co_workers;
        }
        public List<EmployeeExtended> GetReportees(Guid managerId)
        {
            using (var dbx = new PEntities())
            {
                var emps = dbx.Employees.Where(x => x.Manager == managerId && x.Active).ToList();
                List<EmployeeExtended> empsX = new List<EmployeeExtended>();
                if (emps != null)
                {
                    foreach (Employee e in emps)
                    {
                        empsX.Add(new EmployeeExtended(e) { HasReportees = dbx.Employees.Any(z => z.Manager == e.gid && z.Active) });
                    }
                    return empsX.ToList();
                }


            }
            return null;
        }

        public Employee GetManager(Guid empId)
        {
            using (var dbx = new PEntities())
            {
                var mgr = dbx.Employees.SingleOrDefault(x => x.gid == empId);
                return mgr;
            }
        }


        public void SetReportingStructure(Guid guid, Hashtable reportees)
        {
            using (var dbx = new PEntities())
            {
                List<Employee> listOfMatchingEmployees = new List<Employee>();
                foreach (Guid empid in reportees.Keys)
                {
                    var emp = dbx.Employees.SingleOrDefault(x => x.gid == empid);
                    if (emp != null)
                    {
                        listOfMatchingEmployees.Add(emp);
                    }
                }

                if (listOfMatchingEmployees != null)
                {
                    foreach (Employee e in listOfMatchingEmployees)
                    {

                        Employee emp = dbx.Employees.SingleOrDefault(x => x.gid == e.gid);
                        emp.Manager = guid;
                        dbx.SaveChanges();


                    }
                }
            }
        }


        public new Hashtable GetPECycleStatus(Guid empId)
        {
            return base.GetPECycleStatus(empId);
        }









        public List<Employee> OrgStructure()
        {
            using (var dbx = new PEntities())
            {
                return dbx.Employees.Where(x => x.OrgId == OrgId && x.Active).ToList();
            }
        }


        public bool IsFirstTimeUser(Guid employeeid)
        {
            bool? retval = true;
            using (var dbx = new PEntities())
            {
                retval = dbx.Employees.First(x => x.gid == employeeid).FirstTime;
                if (retval.HasValue)
                    return retval.Value;
                else
                {
                    return true;
                }
            }
        }

        public Employee MarkAsExpertUser(Guid employeeid)
        {
            using (var dbx = new PEntities())
            {
                var emp = dbx.Employees.FirstOrDefault(x => x.gid == employeeid);
                if (emp == null)
                {
                    throw new ArgumentException("Employeeid " + employeeid + " does not exist when trying to mark as expert.");
                }
                else
                {
                    emp.FirstTime = false;
                    dbx.SaveChanges();
                }
                return emp;
            }
        }


        public List<Employee> GetReporteesOf(Guid empid)
        {
            using (var dbx = new PEntities())
            {
                var emps = dbx.Employees.Where(x => x.Manager == empid && x.Active);
                if (emps == null)
                {
                    return null;
                }
                else
                {
                    return emps.ToList();
                }
            }
        }


        public Employee GetEmployeeUsingUsername(string username)
        {
            using (PEntities dbx = new PEntities())
            {
                username = username.ToLower();

                if (username.Contains("@"))
                {
                    username = username.Split(new string[] { "@" }, StringSplitOptions.None)[0];
                }
                var emp = dbx.Employees.FirstOrDefault(e => e.Email.ToLower().StartsWith(username.ToLower().Replace("sanjuan\\", "")));
                aspnet_Users user = dbx.aspnet_Users.Where(x => x.LoweredUserName == username.ToLower()).SingleOrDefault();
                if (emp != null)
                {
                    emp.LastVisit = DateTime.Now;
                    dbx.SaveChanges();
                }
                if (emp != null && user != null)
                {
                    if (emp.UserId.ToString().ToLower() != user.UserId.ToString().ToLower())
                    {
                        emp.UserId = null;
                        return emp;
                    }
                }
                else if (emp != null && user == null)
                {
                    emp.UserId = null;
                    return emp;
                }
                else return null;

                return emp;
            }
        }


        public IQueryable<Employee> GetAllEmployees()
        {
            using (PEntities dbx = new PEntities())
            {
                return dbx.Employees.Where(x => x.Active == true);
            }
        }


        public List<EmployeeExtended> GetReviewies(Guid reviewerId)
        {
            using (var dbx = new PEntities())
            {

                List<EmployeeExtended> empsX = new List<EmployeeExtended>();
                if (dbx.Employees.Any(x => x.Reviewer == reviewerId && x.Active))
                {
                    var emps = dbx.Employees.Where(x => x.Reviewer == reviewerId && x.Active).ToList();
                    if (emps != null)
                    {
                        foreach (Employee e in emps)
                        {
                            empsX.Add(new EmployeeExtended(e) { HasReportees = dbx.Employees.Any(z => z.Manager == e.gid && z.Active) });
                        }
                        return empsX.ToList();
                    }
                    return empsX.ToList();
                }


            }
            return null;
        }

        
    }
}
