using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using FluentValidation.Results;

namespace aiof.auth.data
{
    public class AuthFriendlyException : AuthException
    {
        public AuthFriendlyException()
        { }

        public AuthFriendlyException(string message)
            : base(message)
        { }

        public AuthFriendlyException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        { }

        public AuthFriendlyException(HttpStatusCode statusCode, IList<ValidationFailure> failures)
            : base(statusCode, failures)
        { }

        public AuthFriendlyException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}