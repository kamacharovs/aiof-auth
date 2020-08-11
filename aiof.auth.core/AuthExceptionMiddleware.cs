using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.core
{
    public class AuthExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly RequestDelegate _next;

        private const string _defaultMessage = "An unexpected error has occurred";
        private const string _defaultValidationMessage = "One or more validation errors have occurred";

        public AuthExceptionMiddleware(
            ILogger<AuthExceptionMiddleware> logger, 
            IWebHostEnvironment env,
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the http status code middleware will not be executed.");
                    throw;
                }

                var id = string.IsNullOrEmpty(httpContext?.TraceIdentifier)
                    ? Guid.NewGuid().ToString()
                    : httpContext.TraceIdentifier;

                _logger.LogError(
                    e, 
                    $"an exception was thrown during the request. {id}");

                await WriteExceptionResponseAsync(
                    httpContext, 
                    e, 
                    id);
            }
        }

        private async Task WriteExceptionResponseAsync(
            HttpContext httpContext, 
            Exception e, 
            string id)
        {
            var canViewSensitiveInfo = _env
                .IsDevelopment();
            
            var problem = new AuthProblemDetail()
            {
                Message = canViewSensitiveInfo
                    ? e.Message
                    : _defaultMessage,
                Code = StatusCodes.Status500InternalServerError,
                TraceId = $"aiof:auth:error:{id}"
            };

            if (e is AuthException ae)
            {
                problem.Code = ae.StatusCode;
                problem.Message = ae.Message;
            }
            else if (e is ValidationException ve)
            {
                problem.Code = StatusCodes.Status400BadRequest;
                problem.Message = _defaultValidationMessage;
                problem.Errors = ve.Errors.Select(x => x.ErrorMessage);
            }

            var problemjson = JsonSerializer
                .Serialize(problem, new JsonSerializerOptions { IgnoreNullValues = true });

            httpContext.Response.StatusCode = problem.Code ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response
                .WriteAsync(problemjson);
        }
    }

    public static partial class HttpStatusCodeExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthExceptionMiddleware>();
        }
    }
}