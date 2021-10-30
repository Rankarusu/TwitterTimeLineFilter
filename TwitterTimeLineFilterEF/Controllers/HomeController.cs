using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TwitterTimeLineFilterEF.Data;
using TwitterTimeLineFilterEF.Models;

namespace TwitterTimeLineFilterEF.Controllers
//TODO: Maybe use partial views
//TODO: Bind user model to tag assignment page to and generate delete buttons as tags
//TODO: Check if i can keep boxes checked upon reload
//TODO: use http properties
//TODO: load stuff asynchronously at startup, amybe even into database. makes the rest easier maybe.
//TODO: Load friends into db.
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		//View Routing
		public IActionResult Index(List<string> AreChecked)
		{
			var tagcount = AreChecked;

			using (var db = new Models.UserTagContext())
			{
				var tags = db.UserTags.AsEnumerable().OrderBy(x => x.Name).ToList(); //woohoo, using LINQ instead of SQL Queries

				ViewBag.tags = tags;

				if (tagcount.Count == 0)
				{
					ViewBag.tweets = string.Join("\n", db.Tweets.Select(x => x.Html));
				}
				else
				{
					List<string> content = new();

					tags = db.UserTags.Where(x => tagcount.Contains(x.Name)).ToList();
					var users = tags.SelectMany(m => m.Users).ToList();
					content = users.Select(x => x.Name).ToList();

					//var query = TwitterData.ALLTWEETS.Where(x => content.Contains(x.screenname)).Select(x => x.html);
					//var quarey = db.Tweets.Select(x => content.Contains(x.))

					//ViewBag.tweets = string.Join("\n", query);
				}
			}

			return View();
		}

		public IActionResult AssignTags()
		{
			var db = new Models.UserTagContext();

			var tags = db.UserTags.OrderBy(x => x.Name); //woohoo, using LINQ instead of SQL Queries
			var users = db.TwitterUsers.AsEnumerable().OrderBy(x => x.Name);
			ViewBag.tags = tags;
			ViewBag.users = users;

			//TwitterData tdObj = new();
			//ViewBag.friends = tdObj.GetFriends().Result.OrderBy(x => x.ScreenName);


			return View();
		}



		//Methods

		public IActionResult EditTags()
		{
			var db = new Models.UserTagContext();


			ViewBag.tags = db.UserTags.AsEnumerable().OrderBy(x => x.Name); //woohoo, using LINQ instead of SQL Queries

			return View();
		}

		[HttpPost]
		public IActionResult AddNewTag(UserTag tag)
		{
			using (var db = new Models.UserTagContext())
			{
				//TODO: add input validation asp-validation-for is a thing

				db.Add(tag);
				db.SaveChanges();
			}
			return RedirectToAction("EditTags");
		}

		public IActionResult DeleteTag(int Id)
		{
			using (var db = new Models.UserTagContext())
			{
				var TagInDb = db.UserTags.Find(Id);

				if (TagInDb == null)
				{
					return NotFound();
				}

				db.UserTags.Remove(TagInDb);
				db.SaveChanges();
			}
			return RedirectToAction("EditTags");
		}

		[HttpPost]
		public IActionResult AssignTagToUser(string Name, int NewTag)
		{
			using (var db = new Models.UserTagContext())
			{
				var TagInDb = db.UserTags.Find(NewTag);

				var usrObj = db.TwitterUsers.Where(s => s.Name == Name).FirstOrDefault();
				if (usrObj == null)
				{
					var newUser = new TwitterUser { Name = Name };
					db.TwitterUsers.Add(newUser);
					TagInDb.Users.Add(newUser);
				}
				else
				{
					usrObj.Tags.Add(TagInDb);
				}

				db.SaveChanges();
			}
			return RedirectToAction("AssignTags");
		}

		[HttpPost]
		public IActionResult UnassignTagFromUser(string TagName, int UserID)
		{
			using (var db = new Models.UserTagContext())
			{
				var usr = db.TwitterUsers.Find(UserID);
				var tag = db.UserTags.FirstOrDefault(x => x.Name == TagName);

				usr.Tags.Remove(tag);
				db.SaveChanges();
			}
			return RedirectToAction("AssignTags");
		}


		[HttpPost]
		public IActionResult FilterData(List<string> AreChecked)
		{
			return RedirectToAction("Index", new { AreChecked = AreChecked });
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
