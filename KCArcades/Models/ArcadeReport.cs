using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCArcades.Models
{
	public class ArcadeReport
	{
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public int? NumberOfMachines { get; set; }
		public string AdditionalInfo { get; set; }
	}
}