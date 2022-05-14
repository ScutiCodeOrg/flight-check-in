using CrossCuttingCunconers.Infrastructure.Logging;
using CrossCuttingCunconers.Infrastructure.Vault.Extensions;
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
            //host = host.ConfigureDynamicTlsWithVault();
            IWebHost webhost = host.Build();
            webhost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddVault().AddLogging(hostingContext.HostingEnvironment.EnvironmentName);
            })
            .UseSerilog()
            .UseStartup<Startup>();
    }
}
