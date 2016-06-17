using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCArcades.Models;

namespace KCArcades.Controllers
{
    public class ArcadesMapController : Controller
    {
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
			return View();
		}
	}
}