using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouBikeDemo.Models;

namespace YouBikeDemo.ViewModels
{
    public class YouBikeViewModel
    {
        public YouBikeSearchModel SearchParameter { get; set; }

        public IPagedList<YouBike> YouBikes { get; set; }

        public List<SelectListItem> Snos { get; set; }

        public List<SelectListItem> Sareas { get; set; }

        public List<SelectListItem> Snas { get; set; }

        public int PageIndex { get; set; }
    }
}