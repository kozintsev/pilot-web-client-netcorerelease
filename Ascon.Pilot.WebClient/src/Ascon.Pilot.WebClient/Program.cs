using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.Internal.HostingEnvironmentExtensions;

namespace Ascon.Pilot.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.AddNLog();
                    x.AddDebug();
                    //NLog.LogManager.
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // delete all default configuration providers
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.ConfigureApplicationCookie(options =>
                    {
                        options.LoginPath = "/Account/LogIn";
                        options.AccessDeniedPath = "/Home/AccessDenied";
                    });
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                    app.UseStaticFiles();
                })
                .Build();
    }
}
