using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Ascon.Pilot.Web
{
    public class Program
    {
        public static string PilotServerUrl;
        public static string Database;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--help" || args[0] == "-?")
                {
                    Console.WriteLine("");
                    Console.WriteLine("      --connectionsettings [SERVERNAME] [DATABASE]");
                    Console.WriteLine("");
                    Environment.Exit(0);
                }

                if (args[0] == "--connectionsettings" || args[0] == "-c")
                {
                    if (args.Length == 3)
                    {
                        PilotServerUrl = args[1];
                        Database = args[2];
                    }
                    else
                    {
                        Console.WriteLine("Incorrect arguments for --connectionsettings.");
                        Environment.Exit(0);
                    }
                }
            }
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
