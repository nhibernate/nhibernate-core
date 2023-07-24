using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NHibernate.Example.Web
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await CreateWebHostBuilder(args).Build().RunAsync();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
