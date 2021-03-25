using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

using AutoMapper;
using FluentValidation;
using Bogus;
using Moq;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.tests
{
    [ExcludeFromCodeCoverage]
    public class ServiceHelper
    {
        public int? UserId { get; set; }
        public int? ClientId { get; set; }
        public Guid? PublicKey { get; set; }
        public string Token { get; set; }

        public Dictionary<string, string> ConfigurationDict
            => new Dictionary<string, string>()
        {
            { "MemCache:Ttl", "900" },
            { "PostgreSQL", "" },
            { "FeatureManagement:RefreshToken", "false" },
            { "FeatureManagement:OpenId", "false" },
            { "Jwt:Expires", "15" },
            { "Jwt:RefreshExpires", "900" },
            { "Jwt:Type", "Bearer" },
            { "Jwt:Issuer", "aiof:auth" },
            { "Jwt:Audience", "aiof:auth:audience" },
            { "Jwt:PrivateKey", "<RSAKeyValue><Modulus>1EW6wdxMPYBCc/L9RZRSpZx02eSI4YerUl9kHpYk7yFDRArngEZm2ckhQgZFU5BH13JYkfiyB5vLx9L8qZf9w/DtAZewDCaRGWckhGeNtGBDJvCAJaI/PVkwVVOLV/rosbBaqeRjiE4AQl7H+QSPzeidXmf5Zh+otywvtcZqLw8wwPLPFyoqrTeF6naDqxwkGW4E33EwR1qSp2L7RjHJleVbp6EieSsOruekT4QHCVzOfL3C5rz8QmFCPDRycPwuCnB1z0rEm5LWZuDd1z2xFxr3WFgofyEJ+LPicAt/ULrCrj0PB8/f0tMNXGPzj/ZXyerZ3gACX1shLRTDGXxMYQ==</Modulus><Exponent>AQAB</Exponent><P>8UWMH6VUcR//t+hL5zGaQ+EZcGPELt1u+e3wnBCeU1w0yUVqej0Pd/EeK8aK7Ee8y6TrFtelZZJeJEi+WPZ4TCezuNOxS7gQv+Z+BJ82cppHT32r883r8Df+g9JSQMDokJXBmcOcqhZi7kQL94viJ/6HbUL69uIxbxsLN0Vsy8U=</P><Q>4Tr/jBVxOQqo2y6QPUqPYCZektKZExdcxSk9KZSeFD43WglGLJbqk5NrIW6EF7KkbrtroftHByAXjEOfmtVYmw1WCInX2iY0JQHJ/5zvzQl36ERZQPjQqxfWHoZ5pwRrIpREEqWTLIqkEDn2cWDnq5EjkmeqF1wWMQ4cU+gse+0=</Q><DP>m7BXNWyISt9tMg1yPWPiuf6dXcrjI6K8JAcIhoyqvfv4DBEBHpQcUTeEUpcxujqod3iaQwkwr5R9r0hJnDqfcfUEojXoaIYEOPDRTMY4akdn0MG1ngO5Ri+7yvCDJ0nUSMh6P3DcvVzoxfufBUR80XYLidP5Air/30RCPo5MIEk=</DP><DQ>RcPMnEuYAp82po3Jx/Jsbs/zw27QU4WNCtU3SMXsIUEUTCNLRUyJ5KRpLrXY3K5NGK+dK1N2MNLT+HKJ4Z/sDjsXRxXLcpsa08u2EiM+dDhWl+9z5hgsKpL7lAD7dax8sv1SCKSY7dcT8qLBn2Tw3cfbSOIrYYj4psFELQlhnhE=</DQ><InverseQ>Pe6uqbHvu++T28+afeliOsImw+HrMm2f+v8Hq+zIHWkXDlIoTlhT30uNg2DzhIJgRrC+wVm+ZafGqk86d2UldYV93uRRLjmVkAlbfB29WoB8fU9XSbU69FZzGLlkVyGGX5V1nJK0Bf41iiIg14AqsZbhTt1kZoitZhwcrMWcixQ=</InverseQ><D>mn+2EmuZQhocQ/BTz1TJYqihlAX2atAoLegoIur9Lt9y6g9Vt9OGAHWXs0qFIvEcmP8s3/G3Ajqu813pcDakCP1OQ554EB/x9B9SQNxuEx8NQUZCvyF8DdbgivonrX195/aHxqfomcjL/4Kk4eYkFKZ+A7yBlYbtcYTDpPYpSt3b/9wRrKmpr10D+GefZ7gw5uG7C6jqCUTTT16rDeAL1NDB5loCZJVrzGal1yfbdgg+IP4OI6XlF8dJXkYsH2OERsZeB335X6dfYdk2aN+DP4dDxtHReh//OwJiz4kFb/0vGfiF2xZ4Grp/B/TOxz0bcJoQ4tgMYBamDImX7iRpwQ==</D></RSAKeyValue>" },
            { "Jwt:PublicKey", "<RSAKeyValue><Modulus>1EW6wdxMPYBCc/L9RZRSpZx02eSI4YerUl9kHpYk7yFDRArngEZm2ckhQgZFU5BH13JYkfiyB5vLx9L8qZf9w/DtAZewDCaRGWckhGeNtGBDJvCAJaI/PVkwVVOLV/rosbBaqeRjiE4AQl7H+QSPzeidXmf5Zh+otywvtcZqLw8wwPLPFyoqrTeF6naDqxwkGW4E33EwR1qSp2L7RjHJleVbp6EieSsOruekT4QHCVzOfL3C5rz8QmFCPDRycPwuCnB1z0rEm5LWZuDd1z2xFxr3WFgofyEJ+LPicAt/ULrCrj0PB8/f0tMNXGPzj/ZXyerZ3gACX1shLRTDGXxMYQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>" },
            { "Hash:Iterations", "10000" },
            { "Hash:SaltSize", "16" },
            { "Hash:KeySize", "32" }
        };

        public T GetRequiredService<T>()
        {
            var provider = Services().BuildServiceProvider();

            provider.GetRequiredService<FakeDataManager>()
                .UseFakeContext();

            return provider.GetRequiredService<T>();
        }

        public ServiceCollection Services(string token = null)
        {
            var services = new ServiceCollection();

            services.AddScoped<IConfiguration>(x =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddInMemoryCollection(ConfigurationDict)
                    .AddEnvironmentVariables();

                return configurationBuilder.Build();
            });

            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IClientRepository, ClientRepository>()
                .AddScoped<IAuthRepository, AuthRepository>()
                .AddScoped<IUtilRepository, UtilRepository>()
                .AddScoped<FakeDataManager>()
                .AddSingleton<IEnvConfiguration, EnvConfiguration>();

            services.AddScoped(x => GetMockTenant());
            services.AddSingleton(new MapperConfiguration(x => { x.AddProfile(new AutoMappingProfile()); }).CreateMapper());

            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>()
                .AddScoped<AbstractValidator<User>, UserValidator>()
                .AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>()
                .AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>()
                .AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>();

            services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddFeatureManagement();
            services.AddLogging()
                .AddMemoryCache()
                .AddHttpContextAccessor();

            return services;
        }

        public ITenant GetMockTenant()
        {
            var mockedTenant = new Mock<ITenant>();

            var userId = UserId ?? 1;
            var clientId = ClientId ?? 1;
            var publicKey = PublicKey ?? Guid.NewGuid();
            var claims = new Dictionary<string, string> { { AiofClaims.Role, "User" } };
            var token = Token ?? string.Empty;

            mockedTenant.Setup(x => x.UserId).Returns(userId);
            mockedTenant.Setup(x => x.ClientId).Returns(clientId);
            mockedTenant.Setup(x => x.PublicKey).Returns(publicKey);
            mockedTenant.Setup(x => x.Claims).Returns(claims);
            mockedTenant.Setup(x => x.Token).Returns(token);

            return mockedTenant.Object;
        }
    }

    public static class Helper
    {
        #region Unit Tests
        static FakeDataManager _Fake
            => new ServiceHelper().GetRequiredService<FakeDataManager>() ?? throw new ArgumentNullException(nameof(FakeDataManager));

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

        public static IEnumerable<object[]> UsersIdPassword()
        {
            return _Fake.GetFakeUsersData(
                id: true,
                password: true
            );
        }

        public static IEnumerable<object[]> UsersEmailPassword()
        {
            return _Fake.GetFakeUsersData(
                email: true,
                password: true
            );
        }

        public static IEnumerable<object[]> UsersApiKeys()
        {
            return _Fake.GetFakeUsersData(
                apiKeys: true
            );
        }

        public static IEnumerable<object[]> ClientsId()
        {
            return _Fake.GetFakeClientsData(
                id: true
            );
        }

        public static IEnumerable<object[]> ClientsName()
        {
            return _Fake.GetFakeClientsData(
                name: true
            );
        }

        public static IEnumerable<object[]> ClientsApiKey()
        {
            return _Fake.GetFakeClientsData(
                apiKey: true
            );
        }

        public static IEnumerable<object[]> UserRefreshTokensUserId()
        {
            return _Fake.GetFakeUserRefreshTokensData(
                userId: true
            );
        }

        public static IEnumerable<object[]> UserRefreshTokensToken()
        {
            return _Fake.GetFakeUserRefreshTokensData(
                refreshToken: true
            );
        }

        public static IEnumerable<object[]> UserRefreshTokensUserIdToken()
        {
            return _Fake.GetFakeUserRefreshTokensData(
                userId: true,
                refreshToken: true
            );
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

        public static IEnumerable<object[]> RoleNames()
        {
            return _Fake.GetFakeRolesData(
                name: true
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

        public static IEnumerable<object[]> RandomUsers()
        {
            var fakeUsers = new Faker<User>()
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Password, f => _Fake.HashedPassword)
                .Generate(RandomGenerations);

            var toReturn = new List<object[]>();

            foreach (var fakeUser in fakeUsers)
            {
                toReturn.Add(new object[] 
                { 
                    fakeUser.FirstName, 
                    fakeUser.LastName, 
                    fakeUser.Email,
                    fakeUser.Password
                });
            }

            return toReturn;
        }

        public static IEnumerable<UserDto> FakerUserDtos()
        {
            return new Faker<UserDto>()
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Password, f => _Fake.HashedPassword)
                .Generate(RandomGenerations);
        }
        public static IEnumerable<object[]> RandomUserDtos()
        {
            var fakeUserDtos = FakerUserDtos();
            var toReturn = new List<object[]>();

            foreach (var fakeUserDto in fakeUserDtos)
            {
                toReturn.Add(new object[] 
                { 
                    fakeUserDto.FirstName, 
                    fakeUserDto.LastName, 
                    fakeUserDto.Email,
                    fakeUserDto.Password
                });
            }

            return toReturn;
        }

        public static IEnumerable<ClientDto> FakerClientDtos()
        {
            return new Faker<ClientDto>()
                .RuleFor(x => x.Name, f => f.Random.String())
                .RuleFor(x => x.Enabled, f => true)
                .Generate(RandomGenerations);
        }
        public static IEnumerable<object[]> RandomClientDtos()
        {
            var fakeClientDtos = FakerClientDtos();
            var toReturn = new List<object[]>();

            foreach (var fakeClientDto in fakeClientDtos)
            {
                toReturn.Add(new object[] 
                { 
                    fakeClientDto.Name, 
                    fakeClientDto.Enabled
                });
            }

            return toReturn;
        }

        public static IEnumerable<object[]> RandomPasswords()
        {
            return new List<object[]>
            {
                new object[] { "test" },
                new object[] { "Password123" },
                new object[] { "6j1mWDopz8@" },
                new object[] { "F94h7ehL003mHPV934h9B3jpaJ8Wn9wC" }
            };
        }

        public static string ExpiredJwtToken =>
            _Fake.ExpiredJwtToken;


        public const int RandomGenerations = 3;
        public const string Category = nameof(Category);
        public const string UnitTest = nameof(UnitTest);
        public const string IntegrationTest = nameof(IntegrationTest);

        
        public class TestPublicKeyId : IPublicKeyId
        {
            public int Id { get; set; }
            public Guid PublicKey { get; set; } = Guid.NewGuid();
        }
        #endregion
    }
}