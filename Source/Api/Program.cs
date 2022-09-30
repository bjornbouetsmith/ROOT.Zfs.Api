using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)

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
                    //s.AddMvc(o => o.RespectBrowserAcceptHeader = true)
                    //    //.AddXmlSerializerFormatters();
                    //    .AddXmlDataContractSerializerFormatters();

                    
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
                        c.SchemaFilter<EnumSchemaFilter>();
                        c.ParameterFilter<EnumParameterFilter>();
                        //c.UseInlineDefinitionsForEnums();
                    });
                    s.AddControllers();
                    s.AddControllers(options =>
                    {
                        options.Filters.Add(new ProducesAttribute("application/json", "text/json"));//"application/xml", "text/xml",
                        //options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                    });
                });

            return builder;
        }
    }

    public class EnumParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.In.HasValue
                && parameter.In.Value == ParameterLocation.Query
                && context.ParameterInfo.ParameterType.IsEnum)
            {
                var type = context.ParameterInfo.ParameterType;
                var enumerable = Enum.GetNames(type).Select(name => $"{Convert.ToInt64(Enum.Parse(type, name))} - {name}");
                var stringVal = string.Join("<br/>", enumerable);
                parameter.Description += $"<br/><br/>Enumeration - see type:{type.Name}";
                parameter.Description += "<br/><br/>Valid values:<br/>" + string.Join("<br/>", stringVal);
                if (type.GetCustomAttribute<FlagsAttribute>() != null)
                {
                    parameter.Description += "<br/><br/>Flags enumeration, so combinations are also valid";
                }
            }
        }
    }

    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                var enumerable = Enum.GetNames(context.Type).Select(name => $"{Convert.ToInt64(Enum.Parse(context.Type, name))} - {name}");
                var stringVal = string.Join("<br/>", enumerable);
                model.Description += "Valid values:<br/>" + string.Join("<br/>", stringVal);
                // Only make sense to add these values when its not a flags enum
                if (context.Type.GetCustomAttribute<FlagsAttribute>() == null)
                {

                    var enumNames = Enum.GetNames(context.Type).ToList();
                    enumNames.ForEach(name => model.Enum.Add(new OpenApiInteger(Convert.ToInt32(Enum.Parse(context.Type, name)))));
                }
                else
                {
                    model.Description += "<br/><br/>Flags enumeration, so combinations are also valid";
                }
            }
        }
    }
}
