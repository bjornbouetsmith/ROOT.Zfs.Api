using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    
                .ConfigureLogging(conf =>
                {
                    conf.AddDebug();
                    conf.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5000");
                    
                })
                .ConfigureServices(s =>
                    {
                        s.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo
                            {
                                Title = "ZFS API",
                                Version = "v1"
                            });

                            // using System.Reflection;
                            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                        });
                    });
    }
}
