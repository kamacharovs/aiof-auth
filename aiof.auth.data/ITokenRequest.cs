using System;

namespace aiof.auth.data
{
    public interface ITokenRequest<T>
        where T : class
    {
        string ApiKey { get; set; }
        T Entity { get; set; }
    }
}