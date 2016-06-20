using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KCArcades.Models
{
	public class ArcadeReport
	{
		public Arcade Target { get; set; }

		public string TargetName { get; set; }
		public string TargetAddress { get; set; }
		public double TargetLatitude { get; set; }
		public double TargetLongitude { get; set; }

		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public int? NumberOfMachines { get; set; }
		public GPLimitations GPLimitation { get; set; }
		public string AdditionalInfo { get; set; }
	}

	public enum GPLimitations
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
		Infinite = int.MaxValue,
	}
}




