using Microsoft.EntityFrameworkCore;
using System;

namespace TwitterTimeLineFilterEF.Models
{
	public class UserTagContext : DbContext
	{
		public DbSet<TwitterUser> TwitterUsers { get; set; }
		public DbSet<UserTag> UserTags { get; set; }
		public DbSet<Tweet> Tweets { get; set; }

		public string DbPath { get; private set; }

		public UserTagContext()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}db.sqlite3";
		}

		// The following configures EF to create a Sqlite database file in the
		// special "local" folder for your platform.
		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseLazyLoadingProxies().UseSqlite($"Data Source={DbPath}");
	}
}