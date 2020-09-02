using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using aiof.auth.data;

namespace aiof.auth.core
{
    public class AuthUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        private const string _defaultUnauthorizedMessage = "Unauthorized";
        private const string _defaultForbiddenMessage = "Forbidden";

        public AuthUnauthorizedMiddleware(
            RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
            await WriteUnauthorizedResponseAsync(httpContext);
        }

        public async Task WriteUnauthorizedResponseAsync(
            HttpContext httpContext)
        {
            var statusCode = httpContext.Response.StatusCode;
            var authProblem = new AuthProblemDetail();

            if (statusCode == StatusCodes.Status403Forbidden)
            {
                authProblem.Code = StatusCodes.Status403Forbidden;
                authProblem.Message = _defaultForbiddenMessage;
            }

            var authProblemJson = JsonSerializer.Serialize(authProblem);
            httpContext.Response.ContentType = "application/problem+json";
            
            await httpContext.Response
                .WriteAsync(authProblemJson);
        }
    }

    public static partial class HttpStatusCodeExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthUnauthorizedMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthUnauthorizedMiddleware>();
        }
    }
}