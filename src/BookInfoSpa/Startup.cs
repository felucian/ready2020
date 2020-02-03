using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaunchDarkly.Client;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookInfoSpa
{
    public class Startup
    {
        public static LdClient FeatureFlag { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var featureFlagKey = Environment.GetEnvironmentVariable("FEATUREFLAG_KEY");
            FeatureFlag = new LdClient(featureFlagKey);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, TelemetryProperties>();
            var instrumentationKey = Environment.GetEnvironmentVariable("INSTRUMENTATION_KEY");
            services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { DeveloperMode = true, InstrumentationKey = instrumentationKey });
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

			app.Use(async (context, next) => {
				context.Request.PathBase = Configuration["RewritePathBase"];
				await next();
			});

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
			app.UseSession(new SessionOptions()
			{
				IdleTimeout = TimeSpan.FromHours(2)
			});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }

    internal class TelemetryProperties : ITelemetryInitializer
    {
        private IConfiguration configuration;

        public TelemetryProperties(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var versionTag = Environment.GetEnvironmentVariable("VERSIONTAG");
            if (!telemetry.Context.GlobalProperties.Keys.Contains("VersionTag"))
            {
                telemetry.Context.GlobalProperties.Add("VersionTag", versionTag);
            }
        }
    }

    internal static class SessionConstant
    {
        public const string User = "user";
    }

    public static class Flags
    {
        public const string BookInsertNewForm = "Books.Insert.NewForm";
        public const string BookListVisualRating = "Books.List.VisualRating";
    }
}
