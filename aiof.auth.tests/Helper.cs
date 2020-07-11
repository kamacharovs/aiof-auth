using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    public static class Helper
    {
        public static Dictionary<string, string> ConfigurationDict
            => new Dictionary<string, string>()
        {
            { "Token:Ttl", "900" },
            { "Token:Secret", "egSavDYTnYi3M5gWe3g08XQ46S0E2fdh" }
        };

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            provider.GetRequiredService<FakeDataManager>()
                .UseFakeContext();

            return provider.GetRequiredService<T>();
        }

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            services.AddTransient<IConfiguration>(x =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(ConfigurationDict);
                return configurationBuilder.Build();
            });

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<FakeDataManager>();
            services.AddSingleton<IEnvConfiguration, EnvConfiguration>();

            services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddLogging();

            return services.BuildServiceProvider();
        }
    }
}