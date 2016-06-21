using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KCArcades.Models
{
	public class ArcadeReportViewModel
	{
		public int Id { get; set; }
		[Display(Name = "店舗名")]
		public string TargetName { get; set; }
		[Display(Name = "住所")]
		public string TargetAddress { get; set; }
		[Display(Name = "緯度")]
		public double TargetLatitude { get; set; }
		[Display(Name = "経度")]
		public double TargetLongitude { get; set; }

		[Display(Name = "新しい緯度")]
		public double? Latitude { get; set; }
		[Display(Name = "新しい経度")]
		public double? Longitude { get; set; }
		[Display(Name = "設置台数")]
		public int? NumberOfMachines { get; set; }
		[Display(Name = "GP制限")]
		public GPLimitation? GPLimitation { get; set; }
		[Display(Name = "建造回数制限")]
		public int? BuildingLimitation { get; set; }
		[Display(Name = "その他(ハウスルール等)")]
		[DataType(DataType.MultilineText)]
		public string AdditionalInfo { get; set; }
	}

	public enum GPLimitation
	{
		//[Display(Name = "")]
		//None = 0,
		[Display(Name = "300GP")]
		GP300 = 1,
		[Display(Name = "600GP")]
		GP600,
		[Display(Name = "900GP")]
		GP900,
		[Display(Name = "1200GP以上")]
		GP1200Over,
		[Display(Name = "無制限")]
		Infinite = -1,
	}
}




