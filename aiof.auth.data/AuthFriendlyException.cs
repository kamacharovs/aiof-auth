using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace aiof.auth.data
{
    public class AuthFriendlyException : AuthException
    {
        //TODO: add FluentValidation errors []
        public AuthFriendlyException()
        { }

        public AuthFriendlyException(string message)
            : base(message)
        { }

        public AuthFriendlyException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        { }

        public AuthFriendlyException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}