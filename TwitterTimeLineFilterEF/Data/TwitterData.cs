using System;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using TwitterTimelineFilterEF.Data;
using TwitterTimeLineFilterEF.Models;

namespace TwitterTimeLineFilterEF.Data
{
	internal class TwitterData
	{
		public IAuthenticatedUser user;
		public TwitterClient userClient;
		public string username;

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
		public async Task GetTweets()
		{
			var parameters = new GetHomeTimelineParameters();
			parameters.ExcludeReplies = true; //i dont want to see replies

			var homeTimelineTweets = await this.userClient.Timelines.GetHomeTimelineAsync(parameters); //all tweets on your timeline
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
						newTweet.DateTime = tweet.CreatedAt.ToUnixTimeSeconds();
						newTweet.IsRetweet = tweet.IsRetweet;
						await db.Tweets.AddAsync(newTweet);
					}
				}
				await db.SaveChangesAsync();
			}
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
					if (!db.TwitterUsers.Any(x => x.TwitterId == friend.Id)) //add user if not already in there
					{
						TwitterUser newUser = new();
						newUser.TwitterId = friend.Id;
						newUser.Name = friend.ScreenName;
						newUser.DisplayName = friend.Name;
						newUser.ProfileImageUrl = friend.ProfileImageUrl;
						await db.TwitterUsers.AddAsync(newUser);
					}
					else //update existing ones.
					{
						TwitterUser usr = db.TwitterUsers.First(x => x.TwitterId == friend.Id);
						usr.TwitterId = friend.Id;
						usr.Name = friend.ScreenName;
						usr.DisplayName = friend.Name;
						usr.ProfileImageUrl = friend.ProfileImageUrl;
					}
				}
				foreach (var user in db.TwitterUsers) //Delete users no longer followed
				{
					if (friends.Any(x => x.Id == user.Id))
					{
						db.TwitterUsers.Remove(user);
					}
				}
				await db.SaveChangesAsync();
				Console.WriteLine("finished updating users");
			}
		}

		/// <summary>
		/// delete tweets older than 14 days from the database
		/// </summary>
		/// <returns></returns>
		public async Task CleanupTweets()
		{
			var twoWeeksAgo = DateTimeOffset.Now.AddDays(-14).ToUnixTimeSeconds();
			using (var db = new Models.UserTagContext())
			{
				foreach (var tweet in db.Tweets)
				{
					if (tweet.DateTime < twoWeeksAgo)
					{
						db.Remove(tweet);
					}
				}
				await db.SaveChangesAsync();
			}
		}
	}
}