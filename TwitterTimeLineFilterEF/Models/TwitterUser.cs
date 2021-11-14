using System.Collections.Generic;

namespace TwitterTimeLineFilterEF.Models
{
	public class TwitterUser
	{
		public int Id { get; set; }
		public long TwitterId { get; set; }
		public string Name { get; set; } //screenname
		public string DisplayName { get; set; }
		public string ProfileImageUrl { get; set; }

		/* EF Relations */
		public virtual ICollection<UserTag> Tags { get; set; }
		public virtual ICollection<Tweet> Tweets { get; set; }
	}
}