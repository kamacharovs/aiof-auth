using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching;
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
        public IConfiguration _configuration { get; }
        public IWebHostEnvironment _env {get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
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
                services.AddDbContext<AuthContext>(o => o.UseNpgsql(_configuration[Keys.PostgreSQL]));

            services.AddLogging();
            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddMemoryCache();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(_configuration[Keys.OpenApiVersion], new OpenApiInfo
                {
                    Title = _configuration[Keys.OpenApiTitle],
                    Version = _configuration[Keys.OpenApiVersion],
                    Description = _configuration[Keys.OpenApiDescription],
                    Contact = new OpenApiContact
                    {
                        Name = _configuration[Keys.OpenApiContactName],
                        Email = _configuration[Keys.OpenApiContactEmail],
                        Url = new Uri(_configuration[Keys.OpenApiContactUrl])
                    },
                    License = new OpenApiLicense
                    {
                        Name = _configuration[Keys.OpenApiLicenseName],
                        Url = new Uri(_configuration[Keys.OpenApiLicenseUrl]),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });            
            services.AddAuthentication()
                .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = _configuration[Keys.JwtIssuer],
                    ValidAudience = _configuration[Keys.JwtAudience],
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                };
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
