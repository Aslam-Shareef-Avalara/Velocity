using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class FestivalSourceOfficeHoldaysDotCom:BaseModel,IFestivalSource
    {
        public string Url
        {
            get
            {
                return "http://www.officeholidays.com/countries/india/";
            }
            
        }

        public DateTime? GetEventDate(string eventname, string stdeventname, HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                doc.LoadHtml(doc.DocumentNode.OuterHtml.Replace("\n", string.Empty));
                HtmlNode TrNode = null;
                HtmlNode holidayTable = doc.DocumentNode.Descendants("table").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("list-table")).FirstOrDefault();
                var EventRows = holidayTable.Descendants().Where(x => x.Name == "tr" && x.InnerText.Contains(eventname));
                if (EventRows != null && EventRows.Count() > 0)
                {
                    try
                    {
                        TrNode = EventRows.Where(x => x.Descendants().Where(y => y.Name == "td" || y.Name == "th").Skip(2).First().InnerText.ToLower().Trim() == eventname.ToLower()).FirstOrDefault();
                        if (TrNode == null)
                        {
                            TrNode = EventRows.Where(x => x.Descendants().Where(y => y.Name == "td" || y.Name == "th").Skip(2).First().InnerText.ToLower().Trim().Contains(eventname.ToLower())).FirstOrDefault();
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                //HtmlNode eidnode = doc.DocumentNode.SelectSingleNode("//a[contains(.,'" + eventname + "')]");
                //if (eidnode == null)
                //{

                //    eidnode = doc.DocumentNode.SelectSingleNode("//td[contains(.,'" + eventname + "')]");
                //    TrNode = eidnode.ParentNode;
                //}
                //else
                //    TrNode = eidnode.ParentNode.ParentNode;

                string d = TrNode.SelectNodes("td")[1].SelectNodes("span")[0].InnerText;
                int date = int.Parse(d.Split(new string[] { " " }, StringSplitOptions.None)[1]);
                string month = d.Split(new string[] { " " }, StringSplitOptions.None)[0];
                string fulldate = month + " " + date.ToString("0#");
                DateTime dt = DateTime.ParseExact(fulldate, "MMMM d", provider);
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