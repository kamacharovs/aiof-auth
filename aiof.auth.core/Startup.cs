using System;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

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

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUtilRepository, UtilRepository>();
            services.AddScoped<FakeDataManager>();
            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>();
            services.AddScoped<AbstractValidator<User>, UserValidator>();
            services.AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>();
            services.AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>();
            services.AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>();
            services.AddSingleton<IEnvConfiguration, EnvConfiguration>();
            services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);

            if (_env.IsDevelopment())
                services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(nameof(AuthContext)));
            else
                services.AddDbContext<AuthContext>(o => o.UseNpgsql(_config[Keys.PostgreSQL]));
            
            services.AddLogging();
            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddMemoryCache();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                Keys.Bearer,
                x =>
                {
                    var rsa = RSA.Create();
                    rsa.FromXmlString(_config[Keys.JwtPublicKey]);

                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _config[Keys.JwtIssuer],
                        ValidateAudience = true,
                        ValidAudience = _config[Keys.JwtAudience],
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                });

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(_config[Keys.OpenApiVersion], new OpenApiInfo
                {
                    Title = _config[Keys.OpenApiTitle],
                    Version = _config[Keys.OpenApiVersion],
                    Description = _config[Keys.OpenApiDescription],
                    Contact = new OpenApiContact
                    {
                        Name = _config[Keys.OpenApiContactName],
                        Email = _config[Keys.OpenApiContactEmail],
                        Url = new Uri(_config[Keys.OpenApiContactUrl])
                    },
                    License = new OpenApiLicense
                    {
                        Name = _config[Keys.OpenApiLicenseName],
                        Url = new Uri(_config[Keys.OpenApiLicenseUrl]),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

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
                app.UseDeveloperExceptionPage();

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
