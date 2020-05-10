using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YouBikeDemo.Models;
using YouBikeDemo.ViewModels;
using ServiceStack;

namespace YouBikeDemo.Controllers
{
    public class AnotherController : Controller
    {
        const string TargetUri = "http://data.ntpc.gov.tw/NTPC/od/data/api/54DDDC93-589C-4858-9C95-18B2046CC1FC?$format=json";
        const string CacheName = "Auto_YouBike";

        private const int PageSize = 10;

        private Task<List<string>> Snos
        {
            get { return this.GetSnos(); }
        }

        private Task<List<string>> Sareas
        {
            get { return this.GetSareas(); }
        }

        private Task<List<string>> Snas
        {
            get { return this.GetSnas(); }
        }

        public async Task<ActionResult> Index(int page = 1)
        {
            int pageIndex = page < 1 ? 1 : page;

            var source = await this.GetYouBikeData();
            source = source.AsQueryable().OrderBy(x => x.No);

            var model = new YouBikeViewModel
            {
                SearchParameter = new YouBikeSearchModel(),
                PageIndex = pageIndex,
                Snos = this.GetSelectList(await this.Snos, ""),
                Sareas = this.GetSelectList(await this.Sareas, ""),
                Snas = this.GetSelectList(await this.Snas, ""),
                YouBikes = source.ToPagedList(pageIndex, PageSize)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(YouBikeViewModel model)
        {
            int pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;

            var source = await this.GetYouBikeData();
            source = source.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.SearchParameter.No))
            {
                source = source.Where(x => x.No == model.SearchParameter.No);
            }
            if (!string.IsNullOrWhiteSpace(model.SearchParameter.Name))
            {
                source = source.Where(x => x.Name == model.SearchParameter.Name);
            }
            if (!string.IsNullOrWhiteSpace(model.SearchParameter.Area))
            {
                source = source.Where(x => x.Area == model.SearchParameter.Area);
            }

            source = source.OrderBy(x => x.No);

            var result = new YouBikeViewModel
            {
                SearchParameter = model.SearchParameter,
                PageIndex = pageIndex,
                Snos = this.GetSelectList(
                    await this.Snos,
                    model.SearchParameter.No),
                Sareas = this.GetSelectList(
                    await this.Sareas,
                    model.SearchParameter.Name),
                Snas = this.GetSelectList(
                    await this.Snas,
                    model.SearchParameter.Area),
                YouBikes = source.ToPagedList(pageIndex, PageSize)
            };

            return View(result);
        }


        /// <summary>
        /// Gets the hot spot data.
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<YouBike>> GetYouBikeData()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheName))
            {
                var cacheContents = cache.GetCacheItem(CacheName);
                return cacheContents.Value as IEnumerable<YouBike>;
            }
            else
            {
                return await RetriveYouBikeData(CacheName);
            }
        }

        /// <summary>
        /// Retrives the hot spot data.
        /// </summary>
        /// <param name="cacheName">Name of the cache.</param>
        /// <returns></returns>
        private async Task<IEnumerable<YouBike>> RetriveYouBikeData(string cacheName)
        {
            var client = new HttpClient
            {
                MaxResponseContentBufferSize = Int32.MaxValue
            };

            var response = await client.GetStringAsync(TargetUri);

            //=====================================================================================

            var sw = new Stopwatch();
            sw.Start();

            //使用 JSON.Net
            //var collection =
            //    JsonConvert.DeserializeObject<IEnumerable<HotSpot>>(response);

            //使用 ServiceStack.Text (速度比 JSON.Net 快)
            var collection = response.FromJson<IEnumerable<YouBike>>();

            sw.Stop();
            var ts = sw.Elapsed;

            var elapsedTime = String.Format("{0:00}h : {1:00}m :{2:00}s .{3:000}ms",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.WriteLine("RunTime: " + elapsedTime);

            //=====================================================================================

            //資料快取
            ObjectCache cacheItem = MemoryCache.Default;

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(30)
            };

            cacheItem.Add(cacheName, collection, policy);

            return collection;
        }

        /// <summary>
        /// 取得站點代號資料.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetSnos()
        {
            var source = await this.GetYouBikeData();
            if (source == null) return new List<string>();

            var snos = source.OrderBy(x => x.No)
                                  .Select(x => x.No)
                                  .Distinct();

            return snos.ToList();
        }

        /// <summary>
        /// 取得熱點分類.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetSareas()
        {
            var source = await this.GetYouBikeData();
            if (source == null) return new List<string>();

            var sareas = source.OrderBy(x => x.Name)
                              .Select(x => x.Name)
                              .Distinct();

            return sareas.ToList();
        }

        /// <summary>
        /// 取得業者資料.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetSnas()
        {
            var source = await this.GetYouBikeData();
            if (source == null) return new List<string>();

            var snas = source.OrderBy(x => x.Area)
                                 .Select(x => x.Area)
                                 .Distinct();

            return snas.ToList();
        }

        /// <summary>
        /// Gets the select list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selectedItem">The selected item.</param>
        /// <returns></returns>
        private List<SelectListItem> GetSelectList(
            IEnumerable<string> source,
            string selectedItem)
        {
            var selectList = source.Select(item => new SelectListItem()
            {
                Text = item,
                Value = item,
                Selected = !string.IsNullOrWhiteSpace(selectedItem)
                           &&
                           item.Equals(selectedItem, StringComparison.OrdinalIgnoreCase)
            });
            return selectList.ToList();
        }

    }
}