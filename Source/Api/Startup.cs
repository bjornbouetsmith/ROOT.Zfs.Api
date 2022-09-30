using Api.Core;
using Api.Core.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddAuthentication("Basic")
                .AddScheme<BasicAuthSchemeOptions, BasicAuthenticationHandler>("Basic", opt => opt.Realm = "API");

            services.AddResponseCompression();

            if (Configuration.UseLinuxAuth())
            {
                services.AddScoped<IUserService, LinuxPAMUserService>();
            }
            else 
            {
                services.AddScoped<IUserService, StaticUserService>();
            }

            services.AddSingleton<IRemoteConnection>(service => new RemoteConnection(Configuration));
            services.AddScoped<IZfsAccessor, ZfsAccessor>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZFS API v1");
            });
        }
    }
}
