using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YouBikeDemo.Models
{
    public class YouBike
    {
        /// <summary>
        /// 站點代號
        /// </summary>
        [Display(Name = "站點代號")]
        [JsonProperty("sno")]
        public string No { get; set; }

        /// <summary>
        /// 場站名稱(中文)
        /// </summary>
        [Display(Name = "場站名稱(中文)")]
        [JsonProperty("sna")]
        public string Name { get; set; }

        /// <summary>
        /// 場站總停車格
        /// </summary>
        [Display(Name = "場站總停車格")]
        [JsonProperty("tot")]
        public int Total { get; set; }

        /// <summary>
        /// 場站目前車輛數量
        /// </summary>
        [Display(Name = "場站目前車輛數量")]
        [JsonProperty("sbi")]
        public int Bikes { get; set; }

        /// <summary>
        /// 場站區域(中文)
        /// </summary>
        [Display(Name = "場站區域(中文)")]
        [JsonProperty("sarea")]
        public string Area { get; set; }

        /// <summary>
        /// 資料更新時間
        /// </summary>
        [Display(Name = "資料更新時間")]
        [JsonProperty("mday")]
        public string ModifyTime { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        [Display(Name = "緯度")]
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        [Display(Name = "經度")]
        [JsonProperty("lng")]
        public double Longitude { get; set; }

        /// <summary>
        /// 地址(中文)
        /// </summary>
        [Display(Name = "地址")]
        [JsonProperty("ar")]
        public string Address { get; set; }

        /// <summary>
        /// 場站區域(英文)
        /// </summary>
        [Display(Name = "場站區域(英文)")]
        [JsonProperty("sareaen")]
        public string AreaEnglish { get; set; }

        /// <summary>
        /// 場站名稱(英文)
        /// </summary>
        [Display(Name = "場站名稱(英文)")]
        [JsonProperty("snaen")]
        public string NameEnglish { get; set; }

        /// <summary>
        /// 地址(英文)
        /// </summary>
        [Display(Name = "地址(英文)")]
        [JsonProperty("aren")]
        public string AddressEnglish { get; set; }

        /// <summary>
        /// 空位數量
        /// </summary>
        [Display(Name = "空位數量")]
        [JsonProperty("bemp")]
        public int BikeEmpty { get; set; }

        /// <summary>
        /// 禁用狀態 (0:禁用, 1:正常)
        /// </summary>
        [Display(Name = "禁用狀態 (0:禁用, 1:正常)")]
        [JsonProperty("act")]
        public string Active { get; set; }
    }
}
