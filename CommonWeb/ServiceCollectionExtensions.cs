using System;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Sitemap;
using HanumanInstitute.CommonWeb.Utilities;

// ReSharper disable once CheckNamespace - MS guidelines say put DI registration in this NS
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers CommonWeb and CommonWebApp services for ASP.NET.
        /// </summary>
        /// <param name="services">The service container.</param>
        /// <param name="configuration">The application configuration.</param>
        public static IServiceCollection AddCommonWeb(this IServiceCollection services)
        {
            services.CheckNotNull(nameof(services));

            // Services
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IFileSystemService, FileSystemService>();
            services.AddTransient<ISerializationService, SerializationService>();
            services.AddTransient<ISyndicationFeedService, SyndicationFeedService>();
            services.AddTransient<IRandomGenerator, RandomGenerator>();
            services.AddTransient<IWebEnvironment, WebEnvironment>();

            // Utilities
            services.AddTransient<IFormFileHelper, FormFileHelper>();
            services.AddScoped<BlazorQueryString>();

            // Email
            services.AddTransient<IEmailSender, EmailSender>();

            // Sitemap
            services.AddTransient<ISitemapFactory, SitemapFactory>();

            return services;
        }
    }
}
