using DataService;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.ViewModel
{
    public class PeProgressViewModel
    {
        public IPagedList<PeProgressModel> peprogressList = null;
        public int PageCount = 0;
        public int PageNumber = 0;
    }
}