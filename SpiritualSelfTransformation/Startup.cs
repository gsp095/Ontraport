using System;
using System.Diagnostics.CodeAnalysis;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.OntraportApi.Models;
using HanumanInstitute.SpiritualSelfTransformation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.SpiritualSelfTransformation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddRazorPages();

            // Services.
            services.AddCommonWeb();
            services.AddCommonWebApp(_configuration);

            // SpiritualSelfTransformation.
            services.AddOptions<LatestArticlesConfig>()
                .Bind(_configuration.GetSection("LatestArticles"))
                .ValidateDataAnnotations();
            services.AddOptions<AppPathsConfig>()
                .Bind(_configuration.GetSection("AppPaths"))
                .ValidateDataAnnotations();

            services.AddTransient<IUploadHelper, Models.UploadHelper>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Reviewed: Standard function must be a class member to be recognized.")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.CheckNotNull(nameof(env));
            app.CheckNotNull(nameof(app));

            // Validate configuration settings at startup.
            app.ApplicationServices.GetService<IOptions<EmailConfig>>().Value.CheckNotNull("Config: Email");
            app.ApplicationServices.GetService<IOptions<OntraportConfig>>().Value.CheckNotNull("Config: Ontraport");
            app.ApplicationServices.GetService<IOptions<LatestArticlesConfig>>().Value.CheckNotNull("Config: LatestArticles");
            app.ApplicationServices.GetService<IOptions<AppPathsConfig>>().Value.CheckNotNull("Config: AppPaths");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
