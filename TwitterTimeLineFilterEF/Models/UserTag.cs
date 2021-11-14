using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TwitterTimeLineFilterEF.Models
{
	public class UserTag
	{
		public int Id { get; set; }

		[StringLength(20, MinimumLength = 3)]
		public string Name { get; set; }

		/* EF Relations */
		public virtual ICollection<TwitterUser> Users { get; set; }
	}
}