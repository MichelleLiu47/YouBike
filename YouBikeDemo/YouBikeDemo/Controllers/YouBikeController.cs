using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YouBikeDemo.Models;
using System.Runtime.Caching;
using System.Net.Http;

namespace YouBikeDemo.Controllers
{
    public class YouBikeController : Controller
    {
        const string TargetUri = "http://data.ntpc.gov.tw/NTPC/od/data/api/54DDDC93-589C-4858-9C95-18B2046CC1FC?$format=json";
        const string CacheName = "Auto_YouBike";

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="snos">The snos.</param>
        /// <param name="sareas">The saresa.</param>
        /// <param name="snas">The snas.</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(
            int? page,
            string snos,
            string sareas,
            string snas)
        {
            //站點代號
            ViewBag.Snos =
                this.GetSelectList(await this.GetSnos(), snos);
            ViewBag.SelectedSno = snos;

            //中文場站區域分類
            var sareaSelectList =
                this.GetSelectList(await this.GetYouBikeSareas(), sareas);
            ViewBag.Sareas = sareaSelectList.ToList();
            ViewBag.SelectedSarea = sareas;

            //場站名稱
            var snaSelectList =
                this.GetSelectList(await this.GetSnas(), snas);

            ViewBag.Snas = snaSelectList.ToList();
            ViewBag.SelectedSna = snas;

            var source = await this.GetYouBikeData();
            source = source.AsQueryable();

            if (!string.IsNullOrWhiteSpace(snos))
            {
                source = source.Where(x => x.No == snos);
            }
            if (!string.IsNullOrWhiteSpace(sareas))
            {
                source = source.Where(x => x.Area == sareas);
            }
            if (!string.IsNullOrWhiteSpace(snas))
            {
                source = source.Where(x => x.Name == snas);
            }

            int pageIndex = page ?? 1;
            int pageSize = 10;
            int totalCount = 0;

            totalCount = source.Count();

            source = source.OrderBy(x => x.No)
                           .Skip((pageIndex - 1) * pageSize)
                           .Take(pageSize);

            var pagedResult =
                new StaticPagedList<YouBike>(source, pageIndex, pageSize, totalCount);

            return View(pagedResult);
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
        /// 取得場站區域(中文)分類.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetYouBikeSareas()
        {
            var source = await this.GetYouBikeData();
            if (source == null) return new List<string>();

            var sareas = source.OrderBy(x => x.Area)
                              .Select(x => x.Area)
                              .Distinct();

            return sareas.ToList();
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
            var collection =
                JsonConvert.DeserializeObject<IEnumerable<YouBike>>(response);

            //使用 ServiceStack.Text (速度比 JSON.Net 快)
            //var collection = response.FromJson<IEnumerable<HotSpot>>();

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
        /// 取得場站名稱(中文)資料.
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetSnas()
        {
            var source = await this.GetYouBikeData();
            if (source == null) return new List<string>();

            var snas = source.OrderBy(x => x.Name)
                                 .Select(x => x.Name)
                                 .Distinct();

            return snas.ToList();
        }

    }
}