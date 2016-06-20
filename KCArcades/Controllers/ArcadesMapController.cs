using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCArcades.Models;
using System.ComponentModel.DataAnnotations;

namespace KCArcades.Controllers
{
    public class ArcadesMapController : Controller
    {
		static SelectList _GPLimitationList;
		public SelectList GPLimitationList
		{
			get
			{
				return _GPLimitationList ?? (_GPLimitationList = CreateGPLimitationList());
			}
		}
		// GET: ArcadesMap
		public ActionResult Index()
        {
			var directory = Server.MapPath("~/Content/ArcadesJson");
			var files = Directory.GetFiles(directory);
			var allArcades = files.Select(path =>
			{
				string json = null;
				using (var reader = new StreamReader(path))
				{
					json = reader.ReadToEnd();
				}
				return json;
			})
			.Select(json =>
			{
				return JsonConvert.DeserializeObject<List<Arcade>>(json);
			})
			.SelectMany(x => x)
			.ToList();

			var allArcadesJson = JsonConvert.SerializeObject(allArcades);
			ViewBag.Json = allArcadesJson;
			return View();
		}

		[HttpGet]
		public ActionResult Report(Arcade arcade)
		{
			var report = new ArcadeReport
			{
				Target = arcade,
				TargetName = arcade.Name,
				TargetAddress = arcade.Address,
				TargetLatitude = arcade.Latitude,
				TargetLongitude = arcade.Longitude,
				AdditionalInfo = "ハウスルールとか？",
			};

			ViewBag.GPLimitationList = GPLimitationList;

			return View(report);
		}

		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Report(ArcadeReport report)
		{
			// 
			//if (report.Target == null /* || 不正な値  */)
			//{
			//	return View("ReportFailed");
			//}
			if (report.TargetName == null /* || 不正な値  */)
			{
				return View("ReportFailed");
			}

			try
			{
				SendReportMail(report);
			}
			catch
			{
				return View("ReportFailed");
			}

			return View("ReportSuccessful");
		}


		private static SelectList CreateGPLimitationList()
		{
			var type = typeof(GPLimitations);
			var items = Enum.GetValues(type)
				.Cast<GPLimitations>()
				.ToDictionary(p =>
				{
					var attribute = (DisplayAttribute)type.GetField(p.ToString())
						.GetCustomAttributes(typeof(DisplayAttribute), false).First();

					return attribute.Name;
				}, p => (int)p);

			return new SelectList(items, "Value", "Key");
		}

		private static void SendReportMail(ArcadeReport report)
		{
			var sender = "KanColleArcades@kcarcades.net";
			var recipient = "kcarcades-report@gmail.com";
			var subject = "[KCArcades]店舗情報更新リクエスト";

			//店舗名：{ report.Target.Name}
			//住所：{ report.Target.Address}
			//座標：({ report.Target.Latitude},{ report.Target.Longitude})

			var body = $@"
# 更新対象
店舗名：{report.TargetName}
住所：{report.TargetAddress}
座標：({report.TargetLatitude},{report.TargetLongitude})

# 更新データ
設置台数：{report.NumberOfMachines}
GP制限：{report.GPLimitation}
座標：({report.Latitude},{report.Longitude})
追加情報：{report.AdditionalInfo}
";

			using (var client = new System.Net.Mail.SmtpClient())
			{
				client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				client.Send(sender, recipient, subject, body);
			}
		}

	}
}