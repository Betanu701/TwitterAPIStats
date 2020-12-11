using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TwitterAPIStats.Models;

namespace TwitterAPIStats.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}


		[HttpGet]
		public JsonResult GetStats()
		{

			// TweetStatModel TweetStats = new TweetStatModel();
			TweetStatModel TweetStats = TwitterConfig.GetTweetStats();
			return Json(TweetStats, JsonRequestBehavior.AllowGet);
			

		}



	}
}