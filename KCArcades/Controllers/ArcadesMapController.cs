using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCArcades.Models;
using System.ComponentModel.DataAnnotations;
using KCArcades.Models.Tables;
using System.Threading.Tasks;

namespace KCArcades.Controllers
{
    public class ArcadesMapController : AsyncController
    {
		static SelectList _GPLimitationList;
		public SelectList GPLimitationList
		{
			get
			{
				return _GPLimitationList ?? (_GPLimitationList = CreateGPLimitationList());
			}
		}

		private KCArcadesContext db = new KCArcadesContext();
		private ArcadeReportEntityManager manager;

		// GET: ArcadesMap
		public ActionResult Index()
        {
			// ローカルのjsonファイルから読み込む
			//var directory = Server.MapPath("~/Content/ArcadesJson");
			//var files = Directory.GetFiles(directory);
			//var allArcades = files.Select(path =>
			//{
			//	string json = null;
			//	using (var reader = new StreamReader(path))
			//	{
			//		json = reader.ReadToEnd();
			//	}
			//	return json;
			//})
			//.Select(json =>
			//{
			//	return JsonConvert.DeserializeObject<List<ArcadeViewModel>>(json);
			//})
			//.SelectMany(x => x)
			//.ToList();

			// DBから取得
			var arcades = db.Arcades.ToList();
			var allArcades = arcades.Select(x => new ArcadeViewModel
			{
				Id = x.Id,
				Name = x.Name,
				Address = x.Address,
				Latitude = x.Geography.Latitude.Value,
				Longitude = x.Geography.Longitude.Value,
				NumberOfMachines = x.NumberOfMachines.HasValue ? x.NumberOfMachines.Value : new int?(),
				GPLimitation = x.GPLimitation.HasValue ? x.GPLimitation.Value : new GPLimitation?(),
				BuildingLimitation = x.BuildingLimitation.HasValue ? x.BuildingLimitation.Value : new int?(),
				Description = x.Description,
			})
			.ToList();

			var allArcadesJson = JsonConvert.SerializeObject(allArcades);

			ViewBag.Json = allArcadesJson;

			return View();
		}

		[HttpGet]
		public ActionResult Report(int id)
		{
			var arcade = db.Arcades.FirstOrDefault(x => x.Id == id);

			if(arcade == null)
			{
				return View("ReportFailed");
			}

			var report = new ArcadeReportViewModel
			{
				Id = arcade.Id,
				TargetName = arcade.Name,
				TargetAddress = arcade.Address,
				TargetLatitude = arcade.Geography.Latitude.Value,
				TargetLongitude = arcade.Geography.Longitude.Value,
			};

			ViewBag.GPLimitationList = GPLimitationList;

			return View(report);
		}

		[HttpPost]
		[ValidateAntiForgeryToken()]
		public async Task<ActionResult> Report(ArcadeReportViewModel report)
		{
			var arcade = db.Arcades.FirstOrDefault(x => x.Id == report.Id);
			if (arcade == null)
			{
				return View("Index");
			}

			report.TargetName = arcade.Name;
			report.TargetAddress = arcade.Address;
			report.TargetLatitude = arcade.Geography.Latitude.Value;
			report.TargetLongitude = arcade.Geography.Longitude.Value;

			try
			{
				var manager = this.manager ?? (this.manager = await ArcadeReportEntityManager.GetInstanceAsynce());
				var tableResult = await manager.PostReportAsync(report);

				var memo = tableResult == null ? "処理待ち登録失敗" : "処理待ち登録済み";

				SendReportMail(report, memo);
			}
			catch
			{
				var machines = report.NumberOfMachines.HasValue ? $", 台数：{report.NumberOfMachines}" : "";
				var gp = report.NumberOfMachines.HasValue ? $", GP：{report.GPLimitation}" : "";
				var addtional = !string.IsNullOrWhiteSpace(report.AdditionalInfo) ? Environment.NewLine + report.AdditionalInfo : "";
				ViewBag.TweetText = $@"@ticktackmobile [kcarcades更新リクエスト] 【{report.Id}】{report.TargetName}" + machines + gp + addtional;
				
				return View("ReportFailed");
			}

			return View("ReportSuccessful");
		}


		private static SelectList CreateGPLimitationList()
		{
			var type = typeof(GPLimitation);
			var items = Enum.GetValues(type)
				.Cast<GPLimitation>()
				.ToDictionary(p =>
				{
					var attribute = (DisplayAttribute)type.GetField(p.ToString())
						.GetCustomAttributes(typeof(DisplayAttribute), false).First();

					return attribute.Name;
				}, p => (int)p);

			return new SelectList(items, "Value", "Key");
		}

		private static void SendReportMail(ArcadeReportViewModel report, string memo = "")
		{
			var sender = "kcarcades-report@kcarcades.net";
			var subject = "[KCArcades]店舗情報更新リクエスト";

			var body = $@"
# 更新対象
ID：{report.Id}
店舗名：{report.TargetName}
住所：{report.TargetAddress}
座標：({report.TargetLatitude})

# 更新データ
設置台数：{report.NumberOfMachines}
GP制限：{report.GPLimitation}({report.GPLimitation})
座標：({report.Latitude},{report.Longitude})
追加情報：{report.AdditionalInfo}

メモ：{memo}
";

			var username = System.Environment.GetEnvironmentVariable("SENDGRID_USER");
			var password = System.Environment.GetEnvironmentVariable("SENDGRID_PASS");
			var receiver = System.Environment.GetEnvironmentVariable("REPORT_RECEIVER");

			var mailMsg = new System.Net.Mail.MailMessage();
			
			mailMsg.To.Add(new System.Net.Mail.MailAddress(receiver));
			mailMsg.From = new System.Net.Mail.MailAddress(sender);

			mailMsg.Subject = subject;
			mailMsg.IsBodyHtml = false;
			mailMsg.Body = body;

			using (var client = new System.Net.Mail.SmtpClient())
			{
				client.Host = "smtp.sendgrid.net";
				client.Port = 587;
				client.EnableSsl = true;
				client.Credentials = new System.Net.NetworkCredential(username, password);

				client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				client.Send(mailMsg);
			}
		}

	}
}