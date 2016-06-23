using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using KCArcades.Models;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;

namespace KCArcades.Models.Tables
{
	public class ArcadeReportEntityManager
	{
		private static readonly string tableName = "KCArcades";
		private ArcadeReportEntityManager()
		{
		}

		private CloudTable table;

		public static async Task<ArcadeReportEntityManager> GetInstanceAsynce()
		{
			var connectionString = System.Environment.GetEnvironmentVariable("TABLE_CONNECTIONSTRING");

			var storageAccount = CloudStorageAccount.Parse(connectionString);

			var tableClient = storageAccount.CreateCloudTableClient();

			var table = tableClient.GetTableReference(tableName);

			var result = await table.CreateIfNotExistsAsync();

			var manager = new ArcadeReportEntityManager();
			manager.table = table;

			return manager;
		}

		public async Task<ArcadeReportEntity> PostReportAsync(ArcadeReportViewModel report)
		{
			return await PostReportAsync(new ArcadeReportEntity(report));
		}

		public async Task<ArcadeReportEntity> PostReportAsync(ArcadeReportEntity reportEntity)
		{
			return await InsertOrMergeEntityAsync(table, reportEntity);
		}

		private static async Task<ArcadeReportEntity> InsertOrMergeEntityAsync(CloudTable table, ArcadeReportEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

			var result = await table.ExecuteAsync(insertOrMergeOperation);

			var inserted = result.Result as ArcadeReportEntity;

			return inserted;
		}

	}
}