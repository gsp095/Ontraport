using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace HanumanInstitute.CommonWeb.Tests
{
    public static class TestLogger
    {
        public static ILogger<T> Create<T>(ITestOutputHelper output)
        {
            var logger = new TestOutputLogger<T>(output);
            return logger;
        }
    }

    public class TestOutputLogger<T> : ILogger<T>, IDisposable
    {
        private readonly ITestOutputHelper _output;

        public TestOutputLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => _output.WriteLine(formatter?.Invoke(state, exception));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => this;

        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
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
