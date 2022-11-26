using CrossCuttingCunconers.Logging;
using CrossCuttingCunconers.Secrets.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Britannica.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args);
            IWebHost webhost = host.Build();
            webhost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder(args);
            webHostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var configurationRoot = config.AddSecrets().Build();
                configurationRoot.AddLogging();
            });

            webHostBuilder.UseSerilog();

            webHostBuilder.UseStartup<Startup>();

            return webHostBuilder;
        }
    }
}
