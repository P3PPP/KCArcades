﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using KCArcades.Models;

namespace KCArcades.Controllers
{
	public class HomeController : Controller
	{
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
				return JsonConvert.DeserializeObject<List<ArcadeViewModel>>(json);
			})
			.SelectMany(x => x)
			.ToList();

			var allArcadesJson = JsonConvert.SerializeObject(allArcades);
			ViewBag.Json = allArcadesJson;
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}

}