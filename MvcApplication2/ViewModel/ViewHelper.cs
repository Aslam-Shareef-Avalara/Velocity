//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace MvcApplication2.ViewModel
//{

//    public class ViewHelper
//    {
//        public class Festival
//        {
//            public Festival(DateTime start, DateTime end, FestivalType fest)
//            {
//                StartDate = start;
//                EndDate = end;
//                Festivaltype = fest;
//            }

//            public DateTime StartDate;
//            public DateTime EndDate;
//            public FestivalType Festivaltype;
//            ViewHelper vh = new ViewHelper();
//            public string FestivalImage { get { return vh.GetFestivalImage(Festivaltype); } }

//        }
       
//        string GetFestivalImage(FestivalType festival)
//        {
//            switch (festival)
//            {
//                case FestivalType.Christmas: return "~/images/christmas.png";
//                case FestivalType.Diwali: return "~/images/diwali.png";
//                case FestivalType.Eid: return "~/images/eid.png";
//                case FestivalType.New_Year: return "~/images/new_year.png";
//                default: return "~/images/blank.png";
//            }
//        }


//        public static Festival CurrentFestival
//        {
//            get
//            {
//                List<Festival> festivals = new List<Festival>();
//                try
//                {
//                    DateTime DiwaliDate = (DateTime)System.Web.HttpContext.Current.Cache["Diwali"];
//                    DateTime RamadanEid = (DateTime)System.Web.HttpContext.Current.Cache["RamadanEid"];
//                    DateTime Bakrid = (DateTime)System.Web.HttpContext.Current.Cache["Bakrid"];


                    
//                    festivals.Add(new Festival(new DateTime(2999, 12, 30), new DateTime(2999, 12, 30), FestivalType.None));
//                    festivals.Add(new Festival(new DateTime(DateTime.Today.Year, 12, 24), new DateTime(DateTime.Today.Year, 12, 26), FestivalType.Christmas));
//                    festivals.Add(new Festival(DiwaliDate.AddDays(-2), DiwaliDate.AddDays(2), FestivalType.Diwali));
//                    festivals.Add(new Festival(RamadanEid.AddDays(-1), RamadanEid.AddDays(1), FestivalType.Eid));
//                    festivals.Add(new Festival(Bakrid.AddDays(-1), Bakrid.AddDays(1), FestivalType.Eid));
//                    festivals.Add(new Festival(new DateTime(DateTime.Today.Year, 12, 30), new DateTime(DateTime.Today.Month == 12 ? DateTime.Today.Year + 1 : DateTime.Today.Year, 1, 4), FestivalType.New_Year));

//                    DateTime today = DateTime.Today;
//                    foreach (Festival festival in festivals)
//                    {
//                        if (today >= festival.StartDate && today <= festival.EndDate)
//                            return festival;

//                    }

//                    return festivals.FirstOrDefault(x => x.Festivaltype == FestivalType.None);
//                }
//                catch { return festivals.FirstOrDefault(x => x.Festivaltype == FestivalType.None); }

//            }
//        }
//    }
//}