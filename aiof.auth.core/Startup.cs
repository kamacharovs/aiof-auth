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
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

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
            services.AddSingleton<IEnvConfiguration, EnvConfiguration>();
            
            services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);
            
            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>();
            services.AddScoped<AbstractValidator<User>, UserValidator>();
            services.AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>();
            services.AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>();
            services.AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>();

            if (_env.IsDevelopment())
                services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(nameof(AuthContext)));
            else
                services.AddDbContext<AuthContext>(o => o.UseNpgsql(_configuration[Keys.PostgreSQL]));

            services.AddLogging();
            services.AddHealthChecks();
            services.AddFeatureManagement();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1.0.0", new OpenApiInfo
                {
                    Title = "aiof.auth",
                    Version = "v1",
                    Description = "Aiof authentication microservice",
                    Contact = new OpenApiContact
                    {
                        Name = "Georgi Kamacharov",
                        Email = "gkamacharov@aiof.com",
                        Url = new Uri("https://github.com/gkama")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://github.com/kamacharovs/aiof-auth/blob/master/LICENSE"),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            services.AddControllers();
            services.AddMvcCore()
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
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}
