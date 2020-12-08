using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

using FluentValidation;

using aiof.auth.data;
using aiof.auth.services;

namespace aiof.auth.core
{
    public static partial class AuthMiddlewareExtensions
    {
        public static IServiceCollection AddAuthAuthentication(this IServiceCollection services, IEnvConfiguration envConfig)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                Keys.Bearer,
                x =>
                {
                    var rsa = RSA.Create();
                    rsa.FromXmlString(envConfig.JwtPublicKey);

                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = envConfig.JwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = envConfig.JwtAudience,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                });

            return services;
        }

        public static IServiceCollection AddAuthSwaggerGen(this IServiceCollection services, IEnvConfiguration envConfig)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(envConfig.OpenApiVersion, new OpenApiInfo
                {
                    Title = envConfig.OpenApiTitle,
                    Version = envConfig.OpenApiVersion,
                    Description = envConfig.OpenApiDescription,
                    Contact = new OpenApiContact
                    {
                        Name = envConfig.OpenApiContactName,
                        Email = envConfig.OpenApiContactEmail,
                        Url = new Uri(envConfig.OpenApiContactUrl)
                    },
                    License = new OpenApiLicense
                    {
                        Name = envConfig.OpenApiLicenseName,
                        Url = new Uri(envConfig.OpenApiLicenseUrl),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            return services;
        }

        public static IServiceCollection AddAuthFluentValidators(this IServiceCollection services)
        {
            services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>()
                .AddScoped<AbstractValidator<User>, UserValidator>()
                .AddScoped<AbstractValidator<ClientDto>, ClientDtoValidator>()
                .AddScoped<AbstractValidator<AiofClaim>, AiofClaimValidator>()
                .AddScoped<AbstractValidator<TokenRequest>, TokenRequestValidator>();

            return services;
        }

        public static IApplicationBuilder UseAuthExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthExceptionMiddleware>();
        }

        public static IApplicationBuilder UseAuthUnauthorizedMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthUnauthorizedMiddleware>();
        }
    }
}
