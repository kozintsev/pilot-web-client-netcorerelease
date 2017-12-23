﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

namespace Ascon.Pilot.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Debug.WriteLine(args.Length.ToString());
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
