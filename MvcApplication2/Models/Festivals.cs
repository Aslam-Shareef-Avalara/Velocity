using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public interface IFestivalSource
    {
        string Url { get;  }
         DateTime? GetEventDate(string eventname, string stdeventname, HtmlDocument doc);
    }
    public class Festival
    {
        HttpContext ctx = null;
        public Festival(DateTime start, DateTime end, FestivalType fest, string loginview = "login2")
        {
            StartDate = start;
            EndDate = end;
            Festivaltype = fest;
            LoginView = loginview;
        }
        public Festival():this(System.Web.HttpContext.Current) { }
        public Festival(HttpContext ct) { ctx = ct; }
        public string LoginView = "Login2";
        public DateTime StartDate;
        public DateTime EndDate;
        public FestivalType Festivaltype;

        public enum FestivalType
        {
            Christmas,
            Diwali,
            Eid,
            BakriEid,//Dont use this .. this is only for cache key differentiation.
            NewYear,
            Rakhi,
            Independanceday,
            Republicday,
            Holi,
            None
        }
        public Festival GetFestival()
        {
            List<Festival> festivals = new List<Festival>();
            try
            {
                DateTime DiwaliDate = (DateTime)System.Web.HttpContext.Current.Cache[FestivalType.Diwali.ToString().ToLower()];
                DateTime HoliDate = (DateTime)System.Web.HttpContext.Current.Cache[FestivalType.Holi.ToString().ToLower()];
                DateTime RamadanEid = (DateTime)System.Web.HttpContext.Current.Cache[FestivalType.Eid.ToString().ToLower()];
                DateTime Bakrid = (DateTime)System.Web.HttpContext.Current.Cache[FestivalType.BakriEid.ToString().ToLower()];
                DateTime Rakhi = (DateTime)System.Web.HttpContext.Current.Cache[FestivalType.Rakhi.ToString().ToLower()];
                DateTime IndependanceDay = new DateTime(DateTime.Today.Year, 8, 15);
                DateTime RepublicDay = new DateTime(DateTime.Today.Year, 1, 26);

                festivals.Add(new Festival(new DateTime(2999, 12, 30), new DateTime(2999, 12, 30), FestivalType.None, "login2"));
                festivals.Add(new Festival(new DateTime(DateTime.Today.Year, 12, 24), new DateTime(DateTime.Today.Year, 12, 26), FestivalType.Christmas, "christmaslogin"));
                festivals.Add(new Festival(DiwaliDate.AddDays(-2), DiwaliDate.AddDays(2), FestivalType.Diwali, "diwalilogin"));
                festivals.Add(new Festival(RamadanEid.AddDays(-1), RamadanEid.AddDays(1), FestivalType.Eid, "eidlogin"));
                festivals.Add(new Festival(Bakrid.AddDays(-1), Bakrid.AddDays(1), FestivalType.Eid, "eidlogin"));
                festivals.Add(new Festival(new DateTime(DateTime.Today.Year, 12, 30), new DateTime(DateTime.Today.Month == 12 ? DateTime.Today.Year + 1 : DateTime.Today.Year, 1, 4), FestivalType.NewYear));
                festivals.Add(new Festival(Rakhi.AddDays(-1), Rakhi.AddDays(1), FestivalType.Rakhi, "Rakhilogin"));
                festivals.Add(new Festival(IndependanceDay.AddDays(-1), IndependanceDay.AddDays(1), FestivalType.Independanceday, "independencedaylogin"));
                festivals.Add(new Festival(RepublicDay.AddDays(-1), RepublicDay, FestivalType.Republicday, "republicdaylogin"));
                festivals.Add(new Festival(HoliDate.AddDays(-2), HoliDate.AddDays(2), FestivalType.Holi, "holilogin"));

                DateTime today = DateTime.Today;

                Festival currentFestival = null;
                foreach (Festival festival in festivals)
                {
                    if (today >= festival.StartDate && today <= festival.EndDate)
                        return festival;

                }
            }
            catch { }
            return new Festival();
        }

        private string GetStandardEventName(string eventname)
        {
            string stdeventname = "";
            switch (eventname)
            {
                case "Fitr":
                case "Fitar": stdeventname = FestivalType.Eid.ToString().ToLower();
                    break;
                case "Diwali":
                case "Deepavali": stdeventname = FestivalType.Diwali.ToString().ToLower();
                    break;
                case "Adha":
                case "Bakri":
                case "Azha": stdeventname = FestivalType.BakriEid.ToString().ToLower();
                    break;
                case "Rakhi":
                case "Raksha": stdeventname = FestivalType.Rakhi.ToString().ToLower();
                    break;
                case "Holi": stdeventname = FestivalType.Holi.ToString().ToLower();
                    break;
                case "newyear": stdeventname = FestivalType.NewYear.ToString().ToLower();
                    break;
            }

            return stdeventname;
        }

        public DateTime? GetEventDate<T>(string[] forevent) where T: IFestivalSource, new()
        {
            try
            {
                T source = new T();
                string stdeventname = "";
                foreach (string eventname in forevent)
                {
                    stdeventname = GetStandardEventName(eventname);
                    if (ctx.Cache[stdeventname] != null)
                    {
                        return (DateTime)ctx.Cache[stdeventname];
                    }
                }

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(source.Url);
                foreach (string eventname in forevent)
                {
                    stdeventname = GetStandardEventName(eventname);
                    if (ctx.Cache[stdeventname] == null)
                    {

                        DateTime? festivalDate = source.GetEventDate(eventname, stdeventname, doc);
                        if (festivalDate.HasValue)
                            return festivalDate.Value;
                    }
                    else return (DateTime)ctx.Cache[stdeventname];
                }
            }
            catch { }
            return null;
        }

    }
}