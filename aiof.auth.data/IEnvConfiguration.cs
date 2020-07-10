using System;

namespace aiof.auth.data
{
    public interface IEnvConfiguration
    {
        int TokenTtl { get; }
        string TokenSecret { get; }
    }
}