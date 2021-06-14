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
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.core
{
    public static partial class AuthMiddlewareExtensions
    {
        public static IEnvConfiguration _envConfig = Startup._envConfig;

        public static IServiceCollection AddAuthAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                Keys.Bearer,
                x =>
                {
                    var rsa = RSA.Create();
                    rsa.FromXmlString(_envConfig.JwtPublicKey);

                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _envConfig.JwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = _envConfig.JwtAudience,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                });

            return services;
        }

        public static IServiceCollection AddAuthSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(Constants.ApiV1Full, new OpenApiInfo
                {
                    Title = _envConfig.OpenApiTitle,
                    Version = Constants.ApiV1Full,
                    Description = _envConfig.OpenApiDescription,
                    Contact = new OpenApiContact
                    {
                        Name = _envConfig.OpenApiContactName,
                        Email = _envConfig.OpenApiContactEmail,
                        Url = new Uri(_envConfig.OpenApiContactUrl)
                    },
                    License = new OpenApiLicense
                    {
                        Name = _envConfig.OpenApiLicenseName,
                        Url = new Uri(_envConfig.OpenApiLicenseUrl),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            return services;
        }

        public static IServiceCollection AddAuthFluentValidators(this IServiceCollection services)
        {
            services.AddSingleton<AbstractValidator<UserDto>, UserDtoValidator>()
                .AddSingleton<AbstractValidator<ClientDto>, ClientDtoValidator>()
                .AddSingleton<AbstractValidator<AiofClaim>, AiofClaimValidator>()
                .AddSingleton<AbstractValidator<TokenRequest>, TokenRequestValidator>();

            return services;
        }

        public static IServiceCollection AddAuthApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = ApiVersion.Parse(Constants.ApiV1);
                x.ReportApiVersions = true;
                x.ApiVersionReader = new UrlSegmentApiVersionReader();
                x.ErrorResponses = new ApiVersioningErrorResponseProvider();
            });

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

    public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
    {
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            var problem = new AuthProblemDetailBase
            {
                Code = StatusCodes.Status400BadRequest,
                Message = Constants.DefaultUnsupportedApiVersionMessage
            };

            return new ObjectResult(problem)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
