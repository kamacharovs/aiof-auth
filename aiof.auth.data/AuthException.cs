using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace aiof.auth.data
{
    public abstract class AuthException : ApplicationException
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; }

        protected AuthException()
        { }

        protected AuthException(int statusCode)
        {
            StatusCode = statusCode;
        }

        protected AuthException(string message)
            : base(message)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        protected AuthException(string message, Exception inner)
            : base(message, inner)
        { }

        protected AuthException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected AuthException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = (int)statusCode;
        }

        protected AuthException(int statusCode, Exception inner)
            : this(statusCode, inner.ToString())
        { }

        protected AuthException(HttpStatusCode statusCode, Exception inner)
            : this(statusCode, inner.ToString())
        { }

        protected AuthException(int statusCode, JsonElement errorObject)
            : this(statusCode, errorObject.ToString())
        {
            ContentType = @"application/problem+json";
        }
    }
}