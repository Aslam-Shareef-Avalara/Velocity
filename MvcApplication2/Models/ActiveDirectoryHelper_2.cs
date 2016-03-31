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
        private dynamic getPropertyValue(PropertyValueCollection property)
        {

            if (property != null && property.Value != null)
            {
                return property.Value;
            }
            return new object();
        }

        public Employee GetHRManager()
        {
            List<Employee> coworkers = new List<Employee>();
            Employee currentuser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
            string DomainPath = "LDAP://OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com";
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
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
                        using (DirectoryEntry reportee = new DirectoryEntry(result.Path))
                        {
                            return new Employee() { Email = getPropertyValue(reportee.Properties["mail"]), FirstName = getPropertyValue(reportee.Properties["cn"]), LastName = getPropertyValue(reportee.Properties["sn"]), Phone = getPropertyValue(reportee.Properties["telephoneNumber"]) };
                        }
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
            string DomainPath = "LDAP://OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com";
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
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
                    using (DirectoryEntry reportee = new DirectoryEntry(result.Path))
                    {
                        return new Employee() { Email = getPropertyValue(reportee.Properties["mail"]), FirstName = getPropertyValue(reportee.Properties["cn"]), LastName = getPropertyValue(reportee.Properties["sn"]), Phone = getPropertyValue(reportee.Properties["telephoneNumber"].Count > 0 ? reportee.Properties["telephoneNumber"] : reportee.Properties["mobile"]) };
                    }
                }
                catch { }


            }

            return null;
        }

        public List<Employee> GetCoworkers()
        {
            List<Employee> coworkers = new List<Employee>();
            Employee currentuser = System.Web.HttpContext.Current.Session[CONSTANTS.SESSION_CURRENT_USER] as Employee;
            string DomainPath = "LDAP://OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com";
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectClass=user)(objectCategory=person)(department=" + currentuser.Department + "))";
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
                        using (DirectoryEntry reportee = new DirectoryEntry(result.Path))
                        {
                            coworkers.Add(new Employee() { Email = getPropertyValue(reportee.Properties["mail"]), FirstName = getPropertyValue(reportee.Properties["cn"]), LastName = getPropertyValue(reportee.Properties["sn"]), Phone = getPropertyValue(reportee.Properties["mobile"]) });
                        }
                    }
                    catch { }

                }
            }

            return coworkers;
        }

        public int[] OnlineEmployees()
        {
            string DomainPath = "LDAP://OU=Users,OU=Pune Office,DC=SanJuan,DC=Avalara,DC=com";
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
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

            using (DirectorySearcher dsSearcher = new DirectorySearcher())
            {
                dsSearcher.Filter = "(&(objectClass=user) (name=" + name + "))"; //employeeId is the custom column name
                SearchResult result = dsSearcher.FindOne();
                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["objectGUID"].Value)))
                    {
                        byte[] uguid = getPropertyValue(user.Properties["objectGUID"]);
                        string strguid = new Guid(uguid).ToString().ToLower();
                        var c = dbx.Employees.Where(x => x.Manager.Value.ToString().ToLower() == strguid).ToList();
                        if (c.Count > 0)
                            return new Employee { gid = new Guid(getPropertyValue(user.Properties["objectGUID"])) };
                    }
                }
            }
            return null;
        }

        private string getValuePart(string advalue, string advaluepart)
        {
            string[] temp = advalue.Split(new string[] { "," }, StringSplitOptions.None);
            string value = temp.Where(x => x.StartsWith(advaluepart)).FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace(advaluepart + "=", "");
            }
            return value;
        }

        public Employee GetUserDetails()
        {
            Employee _self = new Employee();
            using (DirectorySearcher dsSearcher = new DirectorySearcher())
            {
                dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + System.Web.HttpContext.Current.User.Identity.Name.Replace("SANJUAN\\", "") + "))"; //employeeId is the custom column name
                SearchResult result = dsSearcher.FindOne();
                using (DirectoryEntry user = new DirectoryEntry(result.Path))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["objectGUID"].Value)))
                    {
                        _self.gid = new Guid(getPropertyValue(user.Properties["objectGUID"]));
                        _self.Active = true;
                        _self.Department = getPropertyValue(user.Properties["department"]).ToString();
                        _self.Email = getPropertyValue(user.Properties["mail"]).ToString();
                        _self.FirstName = getPropertyValue(user.Properties["givenName"]).ToString();
                        _self.LastName = getPropertyValue(user.Properties["sn"]).ToString();
                        string OrganizationName = getPropertyValue(user.Properties["company"]).ToString();
                        string managername = getValuePart(getPropertyValue(user.Properties["manager"]).ToString(), "CN");
                        var manager = GetEmployeeFromName(managername);
                        if (manager != null)
                        {
                            _self.Manager = manager.gid;
                        }
                        if (ctx.Session[CONSTANTS.SESSION_ORG_ID] == null)
                        {
                            var org = dbx.Organizations.Where(x => x.Name.Replace(".", "").Replace(",", "") == OrganizationName.Replace(".", "").Replace(",", "")).FirstOrDefault();
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
                return _self;
            }
        }
        public Hashtable GetUserReportees()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["reportees"] != null)
                    return (Hashtable)System.Web.HttpContext.Current.Session["reportees"];

                using (DirectorySearcher dsSearcher = new DirectorySearcher())
                {
                    dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + System.Web.HttpContext.Current.User.Identity.Name.Replace("SANJUAN\\", "") + "))"; //employeeId is the custom column name
                    SearchResult result = dsSearcher.FindOne();
                    Hashtable reporteeNames = new Hashtable();
                    using (DirectoryEntry user = new DirectoryEntry(result.Path))
                    {

                        if (!string.IsNullOrEmpty(Convert.ToString(user.Properties["directReports"].Value)))
                        {
                            object[] ar = (object[])user.Properties["directReports"].Value;
                            foreach (string a in ar)
                            {
                                string[] directreport = a.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                List<string> reportees = directreport.Where(x => x.StartsWith("CN")).ToList();

                                reportees.ForEach(x =>
                                {
                                    dsSearcher.Filter = "(&(objectClass=user) (Name=" + x.Replace("CN=", "") + "))";
                                    result = dsSearcher.FindOne();
                                    using (DirectoryEntry reportee = new DirectoryEntry(result.Path))
                                    {
                                        reporteeNames[new Guid(getPropertyValue(reportee.Properties["objectGUID"]))] = x.Replace("CN=", "");
                                    }


                                });
                            }

                        }
                        System.Web.HttpContext.Current.Session["reportees"] = reporteeNames;
                        return reporteeNames;
                    }
                }
            }
            catch
            {
                return new Hashtable();
            }
        }




    }
}