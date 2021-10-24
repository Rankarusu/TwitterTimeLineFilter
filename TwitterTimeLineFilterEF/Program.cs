using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
			TwitterData tdObj = new();
			await tdObj.getTweets();
			CreateHostBuilder().Build().Run();
		}

		public static IHostBuilder CreateHostBuilder() =>
			Host.CreateDefaultBuilder(new string[0])
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
