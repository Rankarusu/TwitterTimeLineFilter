namespace TwitterTimeLineFilterEF.Models
{
	public class Tweet
	{
		public int Id { get; set; }
		public long TweetId { get; set; }
		public string Html { get; set; }
		public long DateTime { get; set; }
		public bool IsRetweet { get; set; }

		public virtual TwitterUser TwitterUser { get; set; }
	}
}