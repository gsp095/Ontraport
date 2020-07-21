using System;
using System.Net.Http;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.CurrencyExchange;
using HanumanInstitute.CommonWeb.Email;
using HanumanInstitute.CommonWeb.Identity;
using HanumanInstitute.CommonWeb.Ontraport;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.OntraportApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
// using HanumanInstitute.CommonWeb.Validation;
using Polly;

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
        public static IServiceCollection AddCommonWebApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckNotNull(nameof(services));
            configuration.CheckNotNull(nameof(configuration));

            services.AddLazyCache();

            // Configuration
            services.AddOptions<EmailConfig>()
                .Bind(configuration.GetSection("Email"))
                .ValidateDataAnnotations();
            services.AddOptions<CurrencyConverterConfig>()
                .Bind(configuration.GetSection("CurrencyConverter"))
                .ValidateDataAnnotations();
            services.AddOptions<OntraportConfig>()
                .Bind(configuration.GetSection("Ontraport"))
                .ValidateDataAnnotations();
            services.AddOptions<BluePayConfig>()
                .Bind(configuration.GetSection("BluePay"))
                .ValidateDataAnnotations();

            // CurrencyConverter
            //services.AddSingleton<CurrencyConverterHttpClient>();
            //services.AddSingleton<OntraportHttpClient>();
            //services.AddSingleton<BluePayHttpClient>();
            //services.AddHttpClient<CurrencyConverterHttpClient>()
            //services.AddTransient<ICurrencyConverter, CurrencyConverter>();

            services.AddHttpClient<ICurrencyConverter, CurrencyConverter>()
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            // Ontraport
            services.AddOntraportApi(builder =>
                builder.AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)))
            );
            services.AddTransient<IOntraportContacts, OntraportContacts>();
            services.AddTransient<IOntraportReadings, OntraportReadings>();
            services.AddTransient<IOntraportRecordings, OntraportRecordings>();
            services.AddTransient<IOntraportPaymentPlans, OntraportPaymentPlans>();

            // Payment
            services.AddHttpClient<IBluePayProcessor, BluePayProcessor>()
                .ConfigureHttpClient(x => x.DefaultRequestHeaders.UserAgent.ParseAdd(BluePayProcessor.UserAgent))
                .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    CheckCertificateRevocationList = true,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                })
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
            services.AddTransient<IPaymentProcessor, PaymentProcessor>();
            services.AddTransient<IPaymentPlanProcessor, PaymentPlanProcessor>();
            services.AddTransient<IOntraportProcessor, OntraportProcessor>();
            services.AddTransient<ICreditCardProcessor, CreditCardProcessor>();
            services.AddTransient<IPayPalFormProcessor, PayPalFormProcessor>();
            services.AddTransient<IInvoiceSender, InvoiceSender>();

            services.AddTransient<IStaticListsProvider, StaticListsProvider>();

            return services;
        }

        /// <summary>
        /// Registers services for Microsoft Identity Framework.
        /// </summary>
        /// <typeparam name="T">The IdentityUser type.</typeparam>
        /// <typeparam name="TKey">The identity key type.</typeparam>
        /// <param name="services">The service container.</param>
        /// <returns></returns>
        public static IServiceCollection AddCommonIndentity<T, TKey>(this IServiceCollection services)
            where T : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            // Identity
            services.AddTransient<ILogoutManager, LogoutManager<T, TKey>>();

            return services;
        }
    }
}
