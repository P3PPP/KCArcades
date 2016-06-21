using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCArcades.Models
{
	public class ArcadeViewModel
	{
		public int? Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int? NumberOfMachines { get; set; }
		public GPLimitation? GPLimitation { get; set; }
		public string Description { get; set; }
		public int? BuildingLimitation { get; set; }
	}
}