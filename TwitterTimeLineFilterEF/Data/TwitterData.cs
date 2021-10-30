using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterTimelineFilterEF.Data;
using TwitterTimeLineFilterEF.Models;

namespace TwitterTimeLineFilterEF.Data
{
	//class HTMLTweet //a calls to hold all out relevant data
	//{
	//	public string id { get; set; }
	//	public string screenname { get; set; }
	//	public string html { get; set; }

	//	public HTMLTweet(string id, string screenname, string html)
	//	{
	//		this.id = id;
	//		this.screenname = screenname;
	//		this.html = html;
	//	}
	//}
	class TwitterData
	{
		public const int AMOUNT_OF_TWEETS = 0;
		public IAuthenticatedUser user;
		public TwitterClient userClient;
		public string username;
		//public static ConcurrentBag<HTMLTweet> ALLTWEETS = new();

		public TwitterData()
		{
			this.userClient = new TwitterClient(Credentials.APIKey, Credentials.APISecret, Credentials.APIToken, Credentials.APITokenSecret);
			this.user = Task.Run(async () => await userClient.Users.GetAuthenticatedUserAsync()).Result;
			this.username = user.ScreenName;
			Console.WriteLine($"logged in as: {user}");
		}

		/// <summary>
		/// Gets a number of tweets from your home timeline
		/// </summary>
		/// <returns></returns>

		//public async Task<ConcurrentBag<HTMLTweet>> getTweets()
		public async Task GetTweets()
		{
			var homeTimelineTweets = await this.userClient.Timelines.GetHomeTimelineAsync(); //all tweets on your timeline
			Console.WriteLine(homeTimelineTweets.Length + "Tweets found.");

			using (var db = new Models.UserTagContext())
			{
				foreach (var tweet in homeTimelineTweets)
				{
					if (!db.Tweets.Any(x => x.TweetId == tweet.Id))
					{
						Tweet newTweet = new();
						try
						{
							var oEmbedTweet = await userClient.Tweets.GetOEmbedTweetAsync(tweet);
							newTweet.Html = oEmbedTweet.HTML;
						}
						catch (Exception)
						{
							Console.WriteLine("Ran into rate limit.");
							break;
						}

						newTweet.TweetId = tweet.Id;
						newTweet.TwitterUser = db.TwitterUsers.SingleOrDefault(x => x.TwitterId == tweet.CreatedBy.Id);
						await db.Tweets.AddAsync(newTweet);


						//var oEmbedTweet = await userClient.Tweets.GetOEmbedTweetAsync(homeTimelineTweets[i]);
						//var id = homeTimelineTweets[i].IdStr;
						//var screenname = homeTimelineTweets[i].CreatedBy.ScreenName;
						//HTMLTweet htmltweetObj = new(id, screenname, oEmbedTweet.HTML);
						//ALLTWEETS.Add(htmltweetObj);
					}
				}
				await db.SaveChangesAsync();
			}
			//return ALLTWEETS;
		}


		/// <summary>
		/// Syncs database with followed users 
		/// </summary>
		/// <returns></returns>
		public async Task GetFriends()
		{
			var friends = await userClient.Users.GetFriendsAsync(this.username);
			using (var db = new Models.UserTagContext())
			{
				foreach (var friend in friends)
				{
					if (!db.TwitterUsers.Any(x => x.TwitterId == friend.Id))
					{ //add user if not already in there
						TwitterUser newUser = new(); //TODO: try to update existing users an delete the ones unfollowed
						newUser.TwitterId = friend.Id;
						newUser.Name = friend.ScreenName;
						newUser.DisplayName = friend.Name;
						newUser.ProfileImageUrl = friend.ProfileImageUrl;
						await db.TwitterUsers.AddAsync(newUser);
					}
				}
				await db.SaveChangesAsync();
			}

		}
	}
}

