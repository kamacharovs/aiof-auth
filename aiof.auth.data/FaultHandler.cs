using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Linq;

using Polly;

namespace aiof.auth.data
{
    public class FaultHandler
    {
        private readonly IEnvConfiguration _envConfig;

        public FaultHandler(
            IEnvConfiguration envConfig
        )
        {
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
        }

        public void ExecuteWithDefaultAsync<TException>(
            Func<Task> action)
            where TException : Exception
        {
            GetDefaultPolicyAsync<TException>()
                .ExecuteAsync(action);
        }

        private IAsyncPolicy GetDefaultPolicyAsync<TException>()
            where TException : Exception
        {
            return Policy
                .Handle<TException>()
                .RetryAsync(
                    3
                );
        }

        private IAsyncPolicy GetDefaultCircuitBreakerPolicy<TException>(
            Action<Exception, TimeSpan> onBreak,
            Action onReset)
            where TException : Exception
        {
            return Policy
                .Handle<TException>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1), onBreak, onReset);
        }
    }
}