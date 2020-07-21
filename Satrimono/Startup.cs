using System;
using HanumanInstitute.Satrimono.Models;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HanumanInstitute.Satrimono
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.NET Core
            services.AddRazorPages();
            services.AddCommonWeb();
            services.AddCommonWebApp(_configuration);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Database connection
            var connString = _configuration.GetConnectionString("Satrimono").Replace("~", _env.ContentRootPath, StringComparison.InvariantCulture).Replace('\\', System.IO.Path.DirectorySeparatorChar);
            services.AddDbContext<SatrimonoContext>(options => options.UseSqlite(connString));

            // Satrimono
            services.AddOptions<LatestArticlesConfig>()
                .Bind(_configuration.GetSection("LatestArticles"))
                .ValidateDataAnnotations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.CheckNotNull(nameof(app));

            // Validate configuration settings at startup.
            app.ApplicationServices.GetService<IOptions<EmailConfig>>().Value.CheckNotNull("Config: Email");
            app.ApplicationServices.GetService<IOptions<LatestArticlesConfig>>().Value.CheckNotNull("Config: LatestArticles");

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
