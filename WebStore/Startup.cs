using System;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.OntraportApi.Models;
using HanumanInstitute.WebStore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Radzen;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace HanumanInstitute.WebStore
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
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHeadElementHelper();

            // Services
            services.AddCommonWeb();
            services.AddCommonWebApp(Configuration);

            // Radzen controls
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();


            // WebStore
            services.AddSingleton<ProductNames>();

            //services.AddMvc()
            //// services.AddRazorPages()
            //    .AddControllersAsServices().AddViewComponentsAsServices().AddTagHelpersAsServices()
            //    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Reviewed: Standard function can't be static")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            env.CheckNotNull(nameof(env));
            app.CheckNotNull(nameof(app));

            // Validate configuration settings at startup.
            app.ApplicationServices.GetService<IOptions<EmailConfig>>().Value.CheckNotNull("Config: Email");
            app.ApplicationServices.GetService<IOptions<OntraportConfig>>().Value.CheckNotNull("Config: Ontraport");
            app.ApplicationServices.GetService<IOptions<BluePayConfig>>().Value.CheckNotNull("Config: BluePay");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHeadElementServerPrerendering();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                //endpoints.MapRazorPages();
            });
        }
    }
}
