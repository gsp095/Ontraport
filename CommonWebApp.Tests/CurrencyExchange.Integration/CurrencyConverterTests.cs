using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.CommonWeb.Tests;
using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.CommonWeb.CurrencyExchange.IntegrationTests
{
    public class CurrencyConverterTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public Mock<ILogger<CurrencyConverter>> Logger => _logger ??= new Mock<ILogger<CurrencyConverter>>();
        private Mock<ILogger<CurrencyConverter>>? _logger;

        public CurrencyConverterHttpClient Client => _client ??= new CurrencyConverterHttpClient();
        private CurrencyConverterHttpClient? _client;

        public CachingService Cache => _cache ??= new CachingService();
        private CachingService? _cache;

        public ICurrencyConverter Model => _model ??= CreateModel();
        private ICurrencyConverter? _model;
        private ICurrencyConverter CreateModel()
        {
            var config = Options.Create(new CurrencyConverterConfig());
            return new CurrencyConverter(config, Cache, Client, Logger.Object);
        }

        public CurrencyConverterTests(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        [Theory]
        [InlineData(Currency.Cad)]
        [InlineData(Currency.Eur)]
        public async Task GetRateAsync_ValidCurrencies_ReturnsRate(Currency to)
        {
            var result = await Model.GetRateAsync(Currency.Usd, to).ConfigureAwait(false);

            _output.WriteLine(result.ToStringInvariant());
            Assert.NotEqual(1, result);
        }

        [Fact]
        public async Task GetRateAsync_ToUsd_Returns1()
        {
            var result = await Model.GetRateAsync(Currency.Usd, Currency.Usd).ConfigureAwait(false);

            _output.WriteLine(result.ToStringInvariant());
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetRateAsync_ConcurrentRequests_QueriesAndLogsOnce()
        {
            // Nuke the cache as it gets shared between tests.
            Cache.Nuke();

            var task1 = Model.GetRateAsync(Currency.Usd, Currency.Cad);
            var task2 = Model.GetRateAsync(Currency.Usd, Currency.Cad);
            var task3 = Model.GetRateAsync(Currency.Usd, Currency.Cad);
            await Task.WhenAll(new Task<decimal>[] { task1, task2, task3 }).ConfigureAwait(false);

            var r1 = task1.Result;
            var r2 = task2.Result;
            var r3 = task3.Result;
            Logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToStringInvariant().StartsWith("Querying exchange rate from", StringComparison.InvariantCulture)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
            Assert.Equal(task1.Result, task2.Result);
            Assert.Equal(task1.Result, task3.Result);
        }

        [Fact]
        public async Task GetRateAsync_FromUsd_ThrowsException()
        {
            Task Act()
            {
                return Model.GetRateAsync(Currency.Cad, Currency.Usd);
            }

            await Assert.ThrowsAsync<NotSupportedException>(Act).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(Currency.Cad)]
        [InlineData(Currency.Eur)]
        public async Task ConvertAsync_ValidCurrencies_ReturnsDifferentAmount(Currency to)
        {
            var amount = 100;

            var result = await Model.ConvertAsync(amount, Currency.Usd, to).ConfigureAwait(false);

            _output.WriteLine(result.ToStringInvariant());
            Assert.NotEqual(amount, result);
        }

        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
