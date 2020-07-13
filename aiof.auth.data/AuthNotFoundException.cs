using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace aiof.auth.data
{
    public class AuthNotFoundException : AuthFriendlyException
    {
        private const string DefaultMessage = "The requested item was not found.";

        public AuthNotFoundException()
            : base(HttpStatusCode.NotFound, DefaultMessage)
        { }

        public AuthNotFoundException(string message)
            : base(HttpStatusCode.NotFound, message)
        { }

        public AuthNotFoundException(string message, Exception inner)
            : base(message, inner)
        { }

        public AuthNotFoundException(Exception inner)
            : base(DefaultMessage, inner)
        { }
    }
}