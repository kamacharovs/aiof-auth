using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

using FluentValidation.Results;

namespace aiof.auth.data
{
    public class AuthValidationException : AuthException
    {
        [JsonPropertyName("errors")]
        public IEnumerable<string> Errors { get; set; }

        public AuthValidationException(IEnumerable<ValidationFailure> failures)
            : base(HttpStatusCode.BadRequest)
        {
            Errors = failures.Select(x => x.ErrorMessage);
        }
    }
}