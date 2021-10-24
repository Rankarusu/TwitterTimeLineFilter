using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterTimelineFilterEF.Data;

namespace TwitterTimeLineFilterEF.Data
{
    class HTMLTweet //a calls to hold all out relevant data
    {
        public string id { get; set; }
        public string screenname { get; set; }
        public string html { get; set; }

        public HTMLTweet(string id, string screenname, string html)
        {
            this.id = id;
            this.screenname = screenname;
            this.html = html;
        }
    }
    class TwitterData
    {
        public const int AMOUNT_OF_TWEETS = 0;
        public IAuthenticatedUser user;
        public TwitterClient userClient;
        public string username;
        public static ConcurrentBag<HTMLTweet> ALLTWEETS = new();

        public TwitterData()
        {
            this.userClient = new TwitterClient(Credentials.APIKey, Credentials.APISecret, Credentials.APIToken, Credentials.APITokenSecret);
            this.user = Task.Run(async () => await userClient.Users.GetAuthenticatedUserAsync()).Result;
            this.username = user.ScreenName;
            Console.WriteLine($"logged in as: {user}");
        }

        public async Task<ConcurrentBag<HTMLTweet>> getTweets()
        {
            var homeTimelineTweets = await this.userClient.Timelines.GetHomeTimelineAsync(); //all tweets on your timeline


            Console.WriteLine(homeTimelineTweets.Length + "Tweets found.");


            for (int i = 0; i < AMOUNT_OF_TWEETS; i++) //rate limits suck
            {

                var oEmbedTweet = await userClient.Tweets.GetOEmbedTweetAsync(homeTimelineTweets[i]);
                var id = homeTimelineTweets[i].IdStr;
                var screenname = homeTimelineTweets[i].CreatedBy.ScreenName;

                HTMLTweet htmltweetObj = new(id, screenname, oEmbedTweet.HTML);
                ALLTWEETS.Add(htmltweetObj);
            }
            return ALLTWEETS;
        }

        public async Task<IUser[]> getFriends()
        {
            var friends = await userClient.Users.GetFriendsAsync(this.username);
            return friends;
        }
    }
}
