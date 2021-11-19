using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TwitterTimeLineFilterEF.Data;

namespace TwitterTimeLineFilterEF
{
	public class Program
	{
		public static void Main()
		{
			MainAsync().GetAwaiter().GetResult();
		}

		private static async Task MainAsync()
		{
			CreateHostBuilder().Build().Run();
			TwitterData tdObj = new();
			await tdObj.GetTweets();
			await tdObj.GetFriends();
			await tdObj.CleanupTweets();
		}

		public static IHostBuilder CreateHostBuilder() =>
			Host.CreateDefaultBuilder(new string[0])
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}