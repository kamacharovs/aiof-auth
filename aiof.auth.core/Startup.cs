using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<FakeDataManager>();
            services.AddSingleton<IEnvConfiguration, EnvConfiguration>();
            
            services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);
            
            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>();
            services.AddScoped<AbstractValidator<User>, UserValidator>();

            if (_env.IsDevelopment())
                services.AddDbContext<AuthContext>(o => o.UseInMemoryDatabase(nameof(AuthContext)));
            else
                services.AddDbContext<AuthContext>(o => o.UseNpgsql(_configuration["ConnectionString"]));

            services.AddLogging();
            services.AddHealthChecks();

            services.AddControllers();
            services.AddMvcCore()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.WriteIndented = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}
