using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace KCArcades.Models
{
	public class KCArcadesContext : DbContext
	{
		public KCArcadesContext() : base("KCArcadesDb")
		{
		}
		public DbSet<Arcade> Arcades { get; set; }
	}


	public class Arcade
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public DateTime Updated { get; set; }
		public DbGeography Geography { get; set; }
		public int? NumberOfMachines { get; set; }
		public GPLimitation? GPLimitation { get; set; }
		public string Description { get; set; }
		public int? BuildingLimitation { get; set; }
	}
}