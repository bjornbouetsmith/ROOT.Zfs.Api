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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //// configure basic authentication 
            //services.AddAuthentication("Basic")
            //    .AddScheme<BasicAuthSchemeOptions, BasicAuthenticationHandler>("Basic", opt => opt.Realm = "API");
            services.AddAuthentication("Basic")
                .AddScheme<BasicAuthSchemeOptions, BasicAuthenticationHandler>("Basic", opt => opt.Realm = "API");

            //// configure DI for application services
            //services.AddScoped<IUserService, StaticUserService>();
            services.AddScoped<IUserService, LinuxPAMUserService>();

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
        }
    }
}
