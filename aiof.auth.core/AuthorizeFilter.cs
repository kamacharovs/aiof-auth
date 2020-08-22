using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using aiof.auth.services;

namespace aiof.auth.core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute()
            : base(typeof(AuthorizeFilter))
        { }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAuthRepository _repo;

        public AuthorizeFilter(
            IWebHostEnvironment env, 
            IAuthRepository repo)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_env.IsDevelopment())
                return;

            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            var k = 1;

            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Result = new UnauthorizedResult();
        }
    }
}