using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Rms.Server.Test
{
    public class TestLogger<T> : ILogger<T>, IDisposable
    {
        List<TestLog> logs;

        public TestLogger(List<TestLog> logs) : base()
        {
            if (logs == null)
            {
                throw new ArgumentNullException("logs");
            }
            this.logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logs.Add(new TestLog() { LogLevel = logLevel, EventId = eventId, State = state, Exception = exception });
        }

        public void Dispose()
        {
        }
    }
}
