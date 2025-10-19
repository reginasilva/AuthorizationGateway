using Microsoft.Extensions.Logging;

namespace AuthorizationGateway.Api.Tests.Controllers
{
    internal class NoopLogger<T> : ILogger<T>
    {
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        bool ILogger.IsEnabled(LogLevel logLevel) => true;
        
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        
        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }
}