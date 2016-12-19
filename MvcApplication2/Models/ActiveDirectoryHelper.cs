using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Text;
using DataService;
using System.Collections;


namespace MvcApplication2.Models
{
    public class ActiveDirectoryHelper : BaseModel
    {
        DirectoryEntry _de = null;
        const string DomainPath = "LDAP://DC=SanJuan,DC=Avalara,DC=com";//OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com
        public ActiveDirectoryHelper()
        {
            if (ctx != null && ctx.Session["adobj"] != null)
                _de = (DirectoryEntry)ctx.Session["adobj"];
        }
        private dynamic getPropertyValue(PropertyValueCollection property)
        {
            try
            {
                if (property != null && property.Value != null)
                {
                    return property.Value;
                }
            }
            catch
            {

            }
            return new object();
        }

        public Employee GetHRManager()
        {
            List<Employee> coworkers = new List<Employee>();
            Employee currentuser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;

            _de.Path = DomainPath;
            if (_de == null)
                return null;
            DirectorySearcher search = new DirectorySearcher(_de);
            search.Filter = "(&(objectClass=user)(objectCategory=person)(department=HR)(title=Manager*))";
            SearchResult result;
            SearchResult resultCol = search.FindOne();
            if (resultCol != null)
            {

                try
                {

                    string c = (string)resultCol.Properties["distinguishedName"][0];
                    var username = c.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("CN")).FirstOrDefault();
                    username = username.Replace("CN=", "");
                    search.Filter = "(&(objectClass=user) (Name=" + username + "))";
                    result = search.FindOne();
                    DirectoryEntry reportee = _de;
                    reportee.Path = result.Path;
                    Employee HRManager = new Employee();
                    if (!string.IsNullOrEmpty(Convert.ToString(reportee.Properties["objectGUID"].Value)))
                    {
                        byte[] uguid = getPropertyValue(reportee.Properties["objectGUID"]);
                        string strguid = new Guid(uguid).ToString().ToLower();
                        HRManager = dbx.Employees.FirstOrDefault(x => x.gid.ToString().ToLower() == strguid);
                    }
                    reportee.Path = result.Path;
                    return HRManager;
                }
                catch { }


            }

            return null;
        }
        private static string GetBinaryStringFromGuid(string guidstring)
        {
            Guid guid = new Guid(guidstring);

            byte[] bytes = guid.ToByteArray();

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(string.Format(@"\{0}", b.ToString("X")));
            }

            return sb.ToString();
        }
        public Employee GetMyManager()
        {

            List<Employee> coworkers = new List<Employee>();
            Employee currentuser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
            if (currentuser.Manager == null)
                return new Employee();

            DirectoryEntry searchRoot = _de;
            searchRoot.Path = DomainPath;
            DirectorySearcher search = new DirectorySearcher(searchRoot);

            search.Filter = "(&(objectClass=user)(objectCategory=person)(objectGUID=" + GetBinaryStringFromGuid(currentuser.Manager.Value.ToString()) + "))";
            SearchResult result;
            SearchResult resultCol = search.FindOne();
            if (resultCol != null)
            {

                try
                {

                    string c = (string)resultCol.Properties["distinguishedName"][0];
                    var username = c.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("CN")).FirstOrDefault();
                    username = username.Replace("CN=", "");
                    search.Filter = "(&(objectClass=user) (Name=" + username + "))";
                    result = search.FindOne();
                    DirectoryEntry reportee = _de;
                    reportee.Path = result.Path;
                    return new Employee() { Email = getPropertyValue(reportee.Properties["mail"]), FirstName = getPropertyValue(reportee.Properties["cn"]), LastName = getPropertyValue(reportee.Properties["sn"]), Phone = getPropertyValue(reportee.Properties["telephoneNumber"].Count > 0 ? reportee.Properties["telephoneNumber"] : reportee.Properties["mobile"]) };
                }
                catch { }


            }

            return null;
        }

        public List<Employee> GetCoworkers()
        {
            List<Employee> coworkers = new List<Employee>();
            Employee currentuser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
            DirectoryEntry searchRoot = _de;
            searchRoot.Path = DomainPath;
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            searchRoot = _de;
            search.Filter = "(&(objectClass=user)(objectCategory=person)(department=*" + currentuser.Department + "*))";
            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();
            if (resultCol != null)
            {
                for (int counter = 0; counter < resultCol.Count; counter++)
                {
                    try
                    {

                        string c = (string)resultCol[counter].Properties["distinguishedName"][0];
                        var username = c.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.StartsWith("CN")).FirstOrDefault();
                        username = username.Replace("CN=", "");
                        search.Filter = "(&(objectClass=user) (Name=" + username + "))";
                        result = search.FindOne();
                        try
                        {
                            DirectoryEntry reportee = _de;
                            reportee.Path = result.Path;
                            coworkers.Add(new Employee() { Email = getPropertyValue(reportee.Properties["mail"]), FirstName = getPropertyValue(reportee.Properties["cn"]), LastName = getPropertyValue(reportee.Properties["sn"]), Phone = getPropertyValue(reportee.Properties["mobile"]) });
                        }
                        catch (Exception x1)
                        {
                            log.Error("Error in GetCoworkers (" + System.Web.HttpContext.Current.User.Identity.Name +
                                ", REPORTEE:" + username + " ): ", x1);
                        }



                    }
                    catch { }

                }
            }

            return coworkers;
        }

        public int[] OnlineEmployees()
        {
            DirectoryEntry searchRoot = _de;
            searchRoot.Path = DomainPath;
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            search.PropertiesToLoad.Add("lastLogon");
            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();
            int totalOnline = 0;
            int totalEmployees = resultCol.Count;
            if (resultCol != null)
            {
                for (int counter = 0; counter < resultCol.Count; counter++)
                {
                    string UserNameEmailString = string.Empty;
                    result = resultCol[counter];
                    if (result.Properties.Contains("lastLogon"))
                    {

                        DateTime loginTime = DateTime.FromFileTime((long)result.Properties["lastLogon"][0]);
                        if (loginTime > DateTime.Today)
                            totalOnline++;
                    }
                }
            }

            return new int[] { totalEmployees, totalOnline };
        }

        public Employee GetEmployeeFromName(string name)
        {
            DirectoryEntry searchRoot = _de;
            searchRoot.Path = DomainPath;
            DirectorySearcher search = new DirectorySearcher(searchRoot);

            //using (DirectorySearcher dsSearcher = new DirectorySearcher())
            //{
            search.Filter = "(&(objectClass=user) (name=" + name + "))"; //employeeId is the custom column name
            try
            {
                SearchResult result = search.FindOne();

                DirectoryEntry user = _de;
                user.Path = result.Path;
                if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["objectGUID"].Value)))
                {
                    byte[] uguid = getPropertyValue(user.Properties["objectGUID"]);
                    string strguid = new Guid(uguid).ToString().ToLower();
                    return new Employee { gid = new Guid(getPropertyValue(user.Properties["objectGUID"])) };
                }
            }
            catch (Exception x)
            {
                log.Error("When GetEmployeeFromName : " + System.Web.HttpContext.Current.User.Identity.Name, x);
            }
            //}
            return null;
        }

        private string getValuePart(string advalue, string advaluepart)
        {
            try
            {
                string[] temp = advalue.Split(new string[] { "," }, StringSplitOptions.None);
                string value = temp.Where(x => x.StartsWith(advaluepart)).FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace(advaluepart + "=", "");
                }
                return value;
            }
            catch { }
            return string.Empty;
        }

        public Employee GetUserDetails(string username = "")
        {
            DirectoryEntry searchRoot = null;
            if (_de == null)
                return null;
            else
                searchRoot = _de;
            searchRoot.Path = DomainPath;
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            if (string.IsNullOrEmpty(username))
            {
                username = System.Web.HttpContext.Current.User.Identity.Name.Replace("SANJUAN\\", "").Replace("sanjuan\\", "");
            }
            Employee _self = new Employee();
            try
            {
                //using (DirectorySearcher dsSearcher = new DirectorySearcher())
                //{
                search.Filter = "(&(objectClass=user) (sAMAccountName=" + username + "))"; //employeeId is the custom column name
                SearchResult result = search.FindOne();
                DirectoryEntry user = _de;


                user.Path = result.Path;
                if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["objectGUID"].Value)))
                {
                    _self.gid = new Guid(getPropertyValue(user.Properties["objectGUID"]));
                    _self.Active = true;
                    _self.Department = Convert.ToString(getPropertyValue(user.Properties["department"]));
                    _self.Email = Convert.ToString(getPropertyValue(user.Properties["mail"]));
                    _self.FirstName = Convert.ToString(getPropertyValue(user.Properties["givenName"]));
                    _self.Mobile = Convert.ToString(getPropertyValue(user.Properties["mobile"]));
                    _self.Designation = Convert.ToString(getPropertyValue(user.Properties["title"]));
                    _self.LastName = Convert.ToString(getPropertyValue(user.Properties["sn"]));
                    string OrganizationName = Convert.ToString(getPropertyValue(user.Properties["company"]));
                    string managername = getValuePart(Convert.ToString(getPropertyValue(user.Properties["manager"])), "CN");
                    var manager = GetEmployeeFromName(managername);
                    if (manager != null)
                    {
                        _self.Manager = manager.gid;
                    }
                    if (ctx.Session[CONSTANTS.SESSION_ORG_ID] == null || (int)ctx.Session[CONSTANTS.SESSION_ORG_ID] == 0)
                    {
                        Organization org = null;

                        if ((int)ctx.Session[CONSTANTS.SESSION_ORG_ID] == 0)
                            try
                            {
                                org = dbx.Organizations.Where(x => x.Name.Contains("Avalara")).FirstOrDefault();
                            }
                            catch (Exception n)
                            {
                                org = dbx.Organizations.Where(x => x.Name.Replace(".", "").Replace(",", "") == OrganizationName.Replace(".", "").Replace(",", "")).FirstOrDefault();
                            }
                        else 
                            org=dbx.Organizations.Where(x =>  x.Name.Replace(".", "").Replace(",", "") == OrganizationName.Replace(".", "").Replace(",", "")).FirstOrDefault();
                        
                        if (org != null)
                        {
                            _self.OrgId = org.Id;
                        }
                        ctx.Session[CONSTANTS.SESSION_ORG_ID] = org.Id;
                    }
                    else
                        _self.OrgId = (int)ctx.Session[CONSTANTS.SESSION_ORG_ID];

                    _self.Phone = getPropertyValue(user.Properties["mobile"]).ToString();

                }



            }
            catch (Exception x)
            {
                //string self ="";
                //if(_self!=null)
                //self = "_SELF = " + Newtonsoft.Json.JsonConvert.SerializeObject(_self);
                //log.Error("When getting user details : " + "  , "+self+ "STACK TRACE : " + x.StackTrace + " **Inner Exception** " +  x.InnerException!=null? x.InnerException.ToString():"", x);
            }
            return _self;
            //}
        }



        public Hashtable GetUserReportees(string employeename = null)
        {
            try
            {
                DirectoryEntry searchRoot = _de;
                searchRoot.Path = DomainPath;
                DirectorySearcher search = new DirectorySearcher(searchRoot);

                if (System.Web.HttpContext.Current.Session["reportees"] != null && string.IsNullOrEmpty(employeename))
                    return (Hashtable)System.Web.HttpContext.Current.Session["reportees"];

                //using (DirectorySearcher dsSearcher = new DirectorySearcher())
                //{
                if (string.IsNullOrEmpty(employeename))
                    employeename = System.Web.HttpContext.Current.User.Identity.Name.ToLower().Replace("sanjuan\\", "");
                else
                    employeename = employeename.ToLower().Replace("sanjuan\\", "");
                search.Filter = "(&(objectClass=user) (sAMAccountName=" + employeename + "))"; //employeeId is the custom column name
                SearchResult result = search.FindOne();
                Hashtable reporteeNames = new Hashtable();
                DirectoryEntry user = _de;

                user.Path = result.Path;
                if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["directReports"].Value)))
                {

                    object[] ar = (object[])user.Properties["directReports"].Value;

                    foreach (string a in ar)
                    {
                        string[] directreport = a.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> reportees = directreport.Where(x => x.StartsWith("CN")).ToList();

                        reportees.ForEach(x =>
                        {
                            search.SearchRoot.Path = DomainPath;
                            search.Filter = "(&(objectClass=user) (Name=" + x.Replace("CN=", "") + ")(!(UserAccountControl:1.2.840.113556.1.4.803:=2)))";
                            result = search.FindOne();
                            try
                            {
                                DirectoryEntry reportee = _de;
                                reportee.Path = result.Path;
                                reporteeNames[new Guid(getPropertyValue(reportee.Properties["objectGUID"]))] = x.Replace("CN=", "");
                            }
                            catch (Exception x1)
                            {
                                log.Error("Error in GetUserReportees (" + System.Web.HttpContext.Current.User.Identity.Name +
                                    ", REPORTEE:" + x + " ): ", x1);
                            }
                        });
                    }

                }

                System.Web.HttpContext.Current.Session["reportees"] = reporteeNames;
                return reporteeNames;

                //}
            }
            catch (Exception Y)
            {
                log.Error("Error in GetUserReportees : ", Y);
                return new Hashtable();
            }
        }




    }
}