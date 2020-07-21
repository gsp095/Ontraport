using System;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Identity;
using HanumanInstitute.CommonWeb.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HanumanInstitute.RemoteHealingTech.Models;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Radzen;
using Microsoft.AspNetCore.Authentication.Cookies;
using HanumanInstitute.RemoteHealingTech.Services;

using Polly;
using HanumanInstitute.RemoteHealingTech.OntraportSandbox;

namespace HanumanInstitute.RemoteHealingTech
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.NET Core
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHeadElementHelper();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddRouting(options => options.LowercaseUrls = true);

            // Services
            services.AddCommonWeb();
            services.AddCommonWebApp(Configuration);
            services.AddCommonIndentity<ApplicationUser, Guid>();

            // Database connection
            var connString = Configuration.GetConnectionString("RemoteHealingTech").Replace("~", _env.ContentRootPath, StringComparison.InvariantCulture).Replace('\\', System.IO.Path.DirectorySeparatorChar);
            services.AddDbContext<RemoteHealingTechDbContext>(options => options.UseSqlite(connString)
                .EnableSensitiveDataLogging());

            // Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<RemoteHealingTechDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/account/access-denied");
                options.Cookie.Name = "Cookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.LoginPath = new PathString("/account/login");
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            // Radzen
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
          // Identity Services
            services.AddTransient<IUserStore<ApplicationUser>, CustomUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, CustomRoleStore>();
            //Ontraport configuration
            services.Configure<OntraportApi.Models.OntraportConfig>(options => Configuration.Bind("Ontraport", options));
            services.AddOntraportApi();
            services.AddHttpClient<OntraportApi.OntraportHttpClient>().AddTransientHttpErrorPolicy(p =>
            p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
            services.AddTransient<IOntraportContacts, OntraportContacts>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
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
            app.UseRewriter(new RewriteOptions()
                .AddRedirectToHttpsPermanent()
                .AddRedirectToWwwPermanent()
            );
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseHeadElementServerPrerendering();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapRazorPages();
            });
        }
    }
}
