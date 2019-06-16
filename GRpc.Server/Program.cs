using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

// ReSharper disable once IdentifierTypo
namespace GRpc.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}