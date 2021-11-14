using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TwitterTimeLineFilterEF.Models;

namespace TwitterTimeLineFilterEF.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		//View Routing
		public IActionResult Index(List<string> AreChecked, bool ShowRetweets)
		{
			var checkedTags = AreChecked;
			ViewBag.checkedTags = checkedTags;
			bool showRetweets = ShowRetweets;
			ViewBag.ShowRetweets = showRetweets;

			var db = new Models.UserTagContext();

			var allTags = db.UserTags.AsEnumerable().OrderBy(x => x.Name).ToList();
			ViewBag.tags = allTags;

			List<Tweet> tweetsToReturn = new();

			if (checkedTags.Count == 0) //we still need this for the initial 8 posts
			{
				tweetsToReturn = db.Tweets
					.OrderByDescending(x => x.DateTime) //display all tweets when nothing is checked
					.Take(8)
					.ToList();
			}
			else
			{
				var selectedTags = db.UserTags.Where(x => checkedTags.Contains(x.Name));
				var users = selectedTags.SelectMany(m => m.Users);

				tweetsToReturn = db.Tweets
					.Where(x => users.Contains(x.TwitterUser))
					.OrderByDescending(x => x.DateTime)
					.Take(8)
					.ToList();
			}

			if (!showRetweets)
			{
				tweetsToReturn = tweetsToReturn.Where(x => x.IsRetweet == true).ToList();
			}

			ViewBag.tweets = tweetsToReturn;
			return View();
		}

		public IActionResult LoadTweets(long after, List<string> AreChecked, bool ShowRetweets)
		{
			var db = new Models.UserTagContext();
			var checkedTags = AreChecked;
			ViewBag.checkedTags = checkedTags;
			bool showRetweets = ShowRetweets;

			List<Tweet> tweetsToReturn = new();

			if (checkedTags.Count == 0) //we still need this for the initial 8 posts
			{
				tweetsToReturn = db.Tweets
					.OrderByDescending(x => x.DateTime) //display all tweets when nothing is checked
					.Take(8)
					.ToList();
			}
			else
			{
				var selectedTags = db.UserTags.Where(x => checkedTags.Contains(x.Name));
				var users = selectedTags.SelectMany(m => m.Users);

				tweetsToReturn = db.Tweets
					.Where(x => users.Contains(x.TwitterUser))
					.OrderByDescending(x => x.DateTime)
					.Take(8)
					.ToList();
			}

			if (!showRetweets)
			{
				tweetsToReturn = tweetsToReturn.Where(x => x.IsRetweet == true).ToList();
			}

			return PartialView("_Tweet", ViewBag.tweets);
		}

		public IActionResult AssignTags()
		{
			var db = new Models.UserTagContext();

			var tags = db.UserTags.AsEnumerable().OrderBy(x => x.Name);
			var users = db.TwitterUsers.AsEnumerable().OrderBy(x => x.Name);
			ViewBag.tags = tags;
			ViewBag.users = users;
			return View();
		}

		//Methods

		[HttpPost]
		public IActionResult AddNewTag(UserTag tag)
		{
			using (var db = new Models.UserTagContext())
			{
				if (tag.Name != null)
				{
					if (tag.Name.Length > 1 && tag.Name.Length < 20)
					{
						db.Add(tag);
						db.SaveChanges();
					}
				}
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
		public IActionResult FilterData(List<string> AreChecked, bool ShowRetweets)
		{
			return RedirectToAction("Index", new { AreChecked = AreChecked, ShowRetweets = ShowRetweets });
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}