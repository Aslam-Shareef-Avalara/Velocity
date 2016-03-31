using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MvcApplication2.Models
{
    public class OnlineUsers
    {
        public readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(OnlineUsers));
        public List<string> ActiveUsers
        {
            get
            {

                if (HttpRuntime.Cache[CONSTANTS.ONLINE_USERS] != null)
                    return (List<string>)System.Web.HttpContext.Current.Cache[CONSTANTS.ONLINE_USERS];
                else
                {
                    List<string> list = new List<string>();
                    HttpRuntime.Cache.Add(CONSTANTS.ONLINE_USERS, list, null, DateTime.MaxValue, new TimeSpan(), CacheItemPriority.NotRemovable, null);
                    return list;
                }

            }
        }
        public string NewOnlineUser
        {
            set
            {
                List<string> users = ActiveUsers;
                users.Add(value);
            }
        }
        public bool UserOffline(string username)
        {
            try
            {
                if (HttpRuntime.Cache[CONSTANTS.ONLINE_USERS] != null)
                {
                    List<string> users = (List<string>)HttpRuntime.Cache[CONSTANTS.ONLINE_USERS];
                    if (users.Contains(username))
                    {
                        users.Remove(username);
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            catch (Exception x)
            {
                logger.Error("Error when removing user " + username + x.StackTrace, x);
                return false;
            }
        }
    }
}