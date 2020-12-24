using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using aiof.auth.data;

namespace aiof.auth.core
{
    public class AuthUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        private const string _defaultUnauthorizedMessage = "Unauthorized. Missing, invalid or expired credentials provided";
        private const string _defaultForbiddenMessage = "Forbidden. You don't have enough permissions to access this API";
        private IEnumerable<int> _vallowedStatusCodes = new int[] 
        { 
            StatusCodes.Status401Unauthorized, 
            StatusCodes.Status403Forbidden
        };

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
            if (_vallowedStatusCodes.Contains(httpContext.Response.StatusCode) is false)
                return;

            var statusCode = httpContext.Response.StatusCode;
            var authProblem = new AuthProblemDetailBase();

            switch (statusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    authProblem.Code = StatusCodes.Status401Unauthorized;
                    authProblem.Message = _defaultUnauthorizedMessage;
                    break;
                case StatusCodes.Status403Forbidden:
                    authProblem.Code = StatusCodes.Status403Forbidden;
                    authProblem.Message = _defaultForbiddenMessage;
                    break;
            }

            var authProblemJson = JsonSerializer
                .Serialize(authProblem, new JsonSerializerOptions { IgnoreNullValues = true });

            httpContext.Response.ContentType = Keys.ApplicationProblemJson;
            
            await httpContext.Response
                .WriteAsync(authProblemJson);
        }
    }
}