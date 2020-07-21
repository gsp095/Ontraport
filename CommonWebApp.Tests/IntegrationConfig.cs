using System;
using HanumanInstitute.OntraportApi;
using HanumanInstitute.CommonWeb.Payments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.IntegrationTests
{
    public static class IntegrationConfig
    {
        public static OntraportHttpClient GetOntraportHttpClient(ILogger<OntraportHttpClient>? logger = null)
        {
            // var factory = Mock.Of<IHttpClientFactory>(x => x.CreateClient(It.IsAny<string>()) == new HttpClient());
            return new OntraportHttpClient(new HttpClient(), GetOntraportSandBoxConfig(), logger);
        }

        /// <summary>
        /// Returns sandbox account information for Ontraport.
        /// </summary>
        public static IOptions<OntraportConfig> GetOntraportSandBoxConfig()
        {
            var config = new OntraportConfig()
            {
                AppId = "2_200032_vRJdfd92u",
                ApiKey = "JQt4G2decluimoA"
            };
            return Mock.Of<IOptions<OntraportConfig>>(x => x.Value == config);
        }

        /// <summary>
        /// Returns sandbox account information for BluePay.
        /// </summary>
        public static IOptions<BluePayConfig> GetBluePaySandBoxConfig()
        {
            var config = new BluePayConfig()
            {
                AccountIdCad = "100319880297",
                AccountIdUsd = "100319880297",
                SecretKeyCad = "L6WRT.DH2UO5NVVD7ZEJG9NS6/PB6S2N",
                SecretKeyUsd = "L6WRT.DH2UO5NVVD7ZEJG9NS6/PB6S2N",
                TestMode = true
            };
            return Mock.Of<IOptions<BluePayConfig>>(x => x.Value == config);
        }
    }
}
