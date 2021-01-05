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
                .AddSingleton<IEnvConfiguration, EnvConfiguration>()
                .AddScoped<ITenant, Tenant>()
                .AddScoped<FakeDataManager>()
                .AddAutoMapper(typeof(AutoMappingProfile).Assembly)
                .AddAuthFluentValidators();

            services.AddDbContext<AuthContext>(o => o.UseNpgsql(_envConfig.PostgreSQLConnection, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddLogging()
                .AddApplicationInsightsTelemetry()
                .AddHttpContextAccessor()
                .AddMemoryCache()
                .AddAuthAuthentication(_envConfig)
                .AddAuthSwaggerGen(_envConfig);

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
