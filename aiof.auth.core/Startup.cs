using System;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

using AutoMapper;
using FluentValidation;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core
{
    public class Startup
    {
        public readonly IConfiguration _config;
        public readonly IWebHostEnvironment _env;
        public readonly IEnvConfiguration _envConfig;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _envConfig = new EnvConfiguration(_config, null) ?? throw new ArgumentNullException(nameof(EnvConfiguration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IClientRepository, ClientRepository>()
                .AddScoped<IAuthRepository, AuthRepository>()
                .AddScoped<IUtilRepository, UtilRepository>()
                .AddScoped<FakeDataManager>()
                .AddScoped<AbstractValidator<UserDto>, UserDtoValidator>()
                .AddScoped<AbstractValidator<User>, UserValidator>()
                .AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>()
                .AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>()
                .AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>()
                .AddSingleton<IEnvConfiguration, EnvConfiguration>()
                .AddAutoMapper(typeof(AutoMappingProfile).Assembly);

            if (_env.IsDevelopment() && _envConfig.DataInMemory)
                services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(nameof(AuthContext)));
            else
                services.AddDbContext<AuthContext>(o => o.UseNpgsql(_envConfig.DataPostgreSQL, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            
            services.AddLogging();
            services.AddApplicationInsightsTelemetry();
            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddMemoryCache();
            services.AddAuthAuthentication(_envConfig);
            services.AddAuthSwaggerGen(_envConfig);

            services.AddControllers();
            services.AddMvcCore()
                .AddApiExplorer()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.WriteIndented = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    o.JsonSerializerOptions.IgnoreNullValues = true;
                });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider services)
        {
            if (_env.IsDevelopment())
            {
                app.UseCors(x => x.WithOrigins(_envConfig.CorsPortal).AllowAnyHeader().AllowAnyMethod());

                if (_envConfig.DataInMemory)
                    services.GetRequiredService<FakeDataManager>()
                        .UseFakeContext();
            }

            app.UseAuthExceptionMiddleware();
            app.UseAuthUnauthorizedMiddleware();
            app.UseHealthChecks("/health");
            app.UseSwagger();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}
