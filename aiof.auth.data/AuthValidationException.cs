using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using FluentValidation.Results;

namespace aiof.auth.data
{
    public class AuthValidationException : AuthException
    {
        public IEnumerable<string> Errors { get; set; }

        public AuthValidationException(IEnumerable<ValidationFailure> failures)
            : base(HttpStatusCode.BadRequest)
        {
            Errors = failures.Select(x => x.ErrorMessage);
        }
    }
}