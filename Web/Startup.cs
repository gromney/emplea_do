using System;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Web.Framework.Configurations;

namespace Web
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
            Console.WriteLine("Startup.ConfigureServices() Begin");
            services.AddApplicationInsightsTelemetry();
            services.AddFeatureManagement();
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Registers the standard IFeatureManager implementation, which utilizes the .NET Standard configuration system.
            //Read more https://andrewlock.net/introducing-the-microsoft-featuremanagement-library-adding-feature-flags-to-an-asp-net-core-app-part-1/
            
            if (Program.HostingEnvironment.IsDevelopment ())
            {
                //DO NOT CHANGE PLZ THNX <3
                   services.AddDbContext<EmpleaDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            }
            else if(Program.HostingEnvironment.IsProduction())
            {
                services.AddDbContext<EmpleaDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
           
            services.Configure<AppServices.Services.TwitterConfig>(Configuration.GetSection("TwitterConfig"));
            services.Configure<AppServices.Services.TwitterConfig>(Configuration.GetSection("TwitterConfig"));
           
           services.Configure<LegacyApiClient>(Configuration);
           
            IocConfiguration.Init(Configuration, services);
            AuthConfiguration.Init(Configuration, services);

            //services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRazorPages();
            Console.WriteLine("Startup.ConfigureServices() End");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
             Console.WriteLine("Startup.Configure() Begin");
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseAzureAppConfiguration();
            }
            else if (env.IsProduction())
            {
               app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseAzureAppConfiguration();
            }
               
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

         Console.WriteLine("Startup.Configure() End");
        }
    }
}
