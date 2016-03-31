using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class FestivalSourceTimeAndDateDotCom : BaseModel,IFestivalSource
    {
        public string Url
        {
            get
            {
                return "http://www.timeanddate.com/holidays/india/";
            }
            
        }

        public DateTime? GetEventDate(string eventname, string stdeventname, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                HtmlNode eidnode = doc.DocumentNode.SelectSingleNode("//tr[contains(.,'" + eventname + "')]/th");
                int date = int.Parse(eidnode.InnerText.Split(new string[] { " " }, StringSplitOptions.None)[1]);
                string month = eidnode.InnerText.Split(new string[] { " " }, StringSplitOptions.None)[0];
                string fulldate = month + " " + date.ToString("0#");
                DateTime dt = DateTime.ParseExact(fulldate, "MMM d", provider);
                ctx.Cache[stdeventname] = dt;
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}