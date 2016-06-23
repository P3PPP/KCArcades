using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace KCArcades.Models.Tables
{
	public class ArcadeReportEntity : TableEntity
	{
		private static readonly string partitionKey = "Reporting";

		public ArcadeReportEntity(ArcadeReportViewModel report)
		{
			this.PartitionKey = partitionKey;
			this.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d20");
			

			this.ArcadeId = report.Id;

			if (this.NeedsNumberOfMachinesUpdating = report.NumberOfMachines.HasValue)
			{
				this.NumberOfMachines = report.NumberOfMachines;
			}

			if (this.NeedsGPLimitationUpdating = report.GPLimitation.HasValue)
			{
				this.GPLimitation = (int)report.GPLimitation.Value;
			}

			if (this.NeedsBuildingLimitationUpdating = report.BuildingLimitation.HasValue)
			{
				this.BuildingLimitation = report.BuildingLimitation;
			}

			if (this.NeedsGeographyUpdating = (report.Latitude.HasValue && report.Longitude.HasValue))
			{
				this.Geography = $"POINT({report.Latitude.Value} {report.Longitude.Value})";
			}

			if (this.NeedsAdditionalInfoUpdating = string.IsNullOrWhiteSpace(report.AdditionalInfo))
			{
				this.AdditionalInfo = report.AdditionalInfo;
			}
		}

		public int ArcadeId { get; set; }

		public int? NumberOfMachines { get; set; }
		public bool NeedsNumberOfMachinesUpdating { get; set; } = false;

		public int? GPLimitation { get; set; }
		public bool NeedsGPLimitationUpdating { get; set; } = false;

		public int? BuildingLimitation { get; set; }
		public bool NeedsBuildingLimitationUpdating { get; set; } = false;

		public string Geography { get; set; }
		public bool NeedsGeographyUpdating { get; set; } = false;

		public string AdditionalInfo { get; set; }
		public bool NeedsAdditionalInfoUpdating { get; set; } = false;
	}
}