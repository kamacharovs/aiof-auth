using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

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
            { "ConnectionStrings:Database", "" },
            { "FeatureManagement:RefreshToken", "false" },
            { "Jwt:Expires", "15" },
            { "Jwt:RefreshExpires", "900" },
            { "Jwt:Type", "Bearer" },
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
            services.AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>();
            services.AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>();

            services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddLogging();
            services.AddFeatureManagement();

            return services.BuildServiceProvider();
        }

        #region Unit test data
        static FakeDataManager _Fake
            => Helper.GetRequiredService<FakeDataManager>() ?? throw new ArgumentNullException(nameof(FakeDataManager));

        public static IEnumerable<UserDto> RandomUserDtos(int n)
        {
            return _Fake.GetRandomFakeUserDtos(n);
        }

        public static IEnumerable<object[]> UsersId()
        {
            return _Fake.GetFakeUsersData(
                id: true
            );
        }

        public static IEnumerable<object[]> UsersPublicKey()
        {
            return _Fake.GetFakeUsersData(
                publicKey: true
            );
        }

        public static IEnumerable<object[]> UsersUsernamePassword()
        {
            return _Fake.GetFakeUsersData(
                username: true,
                password: true
            );
        }

        public static IEnumerable<object[]> UsersDto()
        {
            return _Fake.GetFakeUserDtosData();
        }

        public static IEnumerable<object[]> ClientsId()
        {
            return _Fake.GetFakeClientsData(
                id: true
            );
        }

        public static IEnumerable<object[]> ClientsApiKey()
        {
            return _Fake.GetFakeClientsData(
                apiKey: true
            );
        }

        public static IEnumerable<object[]> ClientDtos()
        {
            return _Fake.GetFakeClientsDtoData();
        }

        public static IEnumerable<object[]> ClientRefreshClientIdToken()
        {
            return _Fake.GetFakeClientRefreshTokensData(
                clientId: true,
                token: true
            );
        }

        public static IEnumerable<object[]> ClientRefreshToken()
        {
            return _Fake.GetFakeClientRefreshTokensData(
                token: true
            );
        }

        public static IEnumerable<object[]> ApiKeyLength()
        {
            return new List<object[]>
            {
                new object[] { 32 },
                new object[] { 64 },
                new object[] { 128 }
            };
        }

        public static string ExpiredJwtToken =>
            _Fake.ExpiredJwtToken;

        public const string Category = nameof(Category);
        public const string UnitTest = nameof(UnitTest);
        public const string IntegrationTest = nameof(IntegrationTest);
        #endregion
    }
}