using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AttendanceSystemIPCamera.BackgroundServices;
using AttendanceSystemIPCamera.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera
{
    public class Program
    {
        public static int DEFAULT_PORT = 44359;

        public static void Main(string[] args)
        {
            //if (NetworkUtils.IsPortAvailable(DEFAULT_PORT) )
            //{
            //    Console.WriteLine("Port is available");
            //    CreateHostBuilder(args).ConfigureLogging((hostingContext, logging) =>
            //    {
            //        logging.AddConsole();
            //        logging.AddDebug();
            //        logging.AddEventSourceLogger();
            //    }).Build().Run();
            //}
            //else
            //{
            //    Console.WriteLine("Port is in use");
            //}

            //WindowAppRunnerService wpf = new WindowAppRunnerService();
            //wpf.Run();

            //Console.WriteLine("Start successfully!");

            CreateHostBuilder(args).ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    
    }
}
