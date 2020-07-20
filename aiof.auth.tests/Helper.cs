using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    public static class Helper
    {
        public static Dictionary<string, string> ConfigurationDict
            => new Dictionary<string, string>()
        {
            { "Jwt:Expires", "900" },
            { "Jwt:Issuer", "aiof:auth" },
            { "Jwt:Audience", "aiof:auth:audience" },
            { "Jwt:Secret", "egSavDYTnYi3M5gWe3g08XQ46S0E2fdh" },
            { "Hash:Iterations", "10000" },
            { "Hash:SaltSize", "16" },
            { "Hash:KeySize", "32" }
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
                configurationBuilder.AddEnvironmentVariables();
                return configurationBuilder.Build();
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<FakeDataManager>();
            services.AddSingleton<IEnvConfiguration, EnvConfiguration>();

            services.AddSingleton(new MapperConfiguration(x => { x.AddProfile(new AutoMappingProfile()); }).CreateMapper());

            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>();
            services.AddScoped<AbstractValidator<User>, UserValidator>();
            services.AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>();

            services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddLogging();

            return services.BuildServiceProvider();
        }

        #region TestData
        static FakeDataManager _Fake
            => Helper.GetRequiredService<FakeDataManager>() ?? throw new ArgumentNullException(nameof(FakeDataManager));

        public static IEnumerable<object[]> UsersId()
        {
            return _Fake.GetFakeUsersData(
                id: true
            );
        }

        public static IEnumerable<object[]> UsersApiKey()
        {
            return _Fake.GetFakeUsersData(
                apiKey: true
            );
        }

        public static IEnumerable<object[]> UsersIdApiKey()
        {
            return _Fake.GetFakeUsersData(
                id: true,
                apiKey: true
            );
        }

        public static IEnumerable<object[]> UsersUsernamePassword()
        {
            return _Fake.GetFakeUsersData(
                username: true,
                password: true
            );
        }

        public static IEnumerable<object[]> ClientDtos()
        {
            return _Fake.GetFakeClientsDtoData();
        }

        public static string ExpiredJwtToken =>
            _Fake.ExpiredJwtToken;
        #endregion
    }
}