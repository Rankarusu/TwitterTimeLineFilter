using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TwitterTimeLineFilterEF.Models;

namespace TwitterTimeLineFilterEF.Controllers
//TODO: Maybe use partial views
//TODO: use http properties
//TODO: lazy loading on scroll

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
			var checkedTags = AreChecked;
			ViewBag.checkedTags = checkedTags;

			var db = new Models.UserTagContext();

			var allTags = db.UserTags.AsEnumerable().OrderBy(x => x.Name).ToList(); //woohoo, using LINQ instead of SQL Queries
			ViewBag.tags = allTags; //all tags

			if (checkedTags.Count == 0) //we still need this for the initial 8 posts
			{
				ViewBag.tweets = db.Tweets.Take(8).ToList().OrderByDescending(x => x.DateTime); //display all tweets when nothing is checked
			}
			else
			{
				var selectedTags = db.UserTags.Where(x => checkedTags.Contains(x.Name));
				var users = selectedTags.SelectMany(m => m.Users);

				ViewBag.tweets = db.Tweets.Where(x => users.Contains(x.TwitterUser)).OrderByDescending(x => x.DateTime).Take(5);
			}

			return View();
		}
		public IActionResult LoadTweets(long after, List<string> AreChecked)
		{
			var db = new Models.UserTagContext();
			var checkedTags = AreChecked;


			if (checkedTags.Count == 0)
			{
				ViewBag.tweets = db.Tweets
				.OrderByDescending(x => x.DateTime)
				.Where(t => t.DateTime < after)
				.Take(8)
				.ToList();
			}
			else
			{
				var selectedTags = db.UserTags.Where(x => checkedTags.Contains(x.Name));
				var users = selectedTags.SelectMany(m => m.Users);

				ViewBag.tweets = db.Tweets
					.OrderByDescending(x => x.DateTime)
					.Where(x => users.Contains(x.TwitterUser))
					.Where(t => t.DateTime < after)
					.Take(8)
					.ToList();
			}



			return PartialView("_Tweet", ViewBag.tweets);
		}

		public IActionResult AssignTags()
		{
			var db = new Models.UserTagContext();

			var tags = db.UserTags.AsEnumerable().OrderBy(x => x.Name); //woohoo, using LINQ instead of SQL Queries
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
			db.Dispose();
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
			return RedirectToAction("AssignTags");
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
			return RedirectToAction("AssignTags");
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