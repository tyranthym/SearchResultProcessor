using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SearchResultProcessor.Services;
using System.IO;
using System.Reflection;

namespace SearchResultProcessor
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
            //here using selenium web driver to 100% simulate opening chrome browser - get the html after js/css executed
            var outputDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var chromeDriverOptions = new ChromeOptions();
            chromeDriverOptions.AddArgument("--lang=en-au");
            chromeDriverOptions.AddArgument("--no-sandbox");
            chromeDriverOptions.AddArgument("--headless");
            services.AddScoped<IWebDriver, ChromeDriver>(x => new ChromeDriver(outputDirectory, chromeDriverOptions));

            //fluent validation
            var assembly = Assembly.GetAssembly(typeof(Startup));

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(assembly))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  //to prevent reference loops from happening
                    options.SerializerSettings.Formatting = Formatting.Indented;    //For pretty print Swagger JSON
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            //app services
            services.AddScoped<IScrapingService, GoogleScrapingService>();

            services.AddProblemDetails();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseProblemDetails();

            app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
