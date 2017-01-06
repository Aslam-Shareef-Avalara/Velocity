using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models 
{
    public class FestivalSourceCalendarLabsDotCom : BaseModel, IFestivalSource
    {
        public string Url
        {
            get { return "http://www.calendarlabs.com/online-calendar/2017-calendar/india-holidays/"; }
        }

        public DateTime? GetEventDate(string eventname, string stdeventname, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                HtmlNode holidayDateNode = doc.DocumentNode.SelectSingleNode("//td[contains(translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'" + eventname.ToLower() + "') and contains(@class ,'calendarbody2')]/preceding::td[1]");
                var datesplit = holidayDateNode.InnerText.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int date = int.Parse(datesplit[1]);
                string month = datesplit[0];
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