using System;

namespace aiof.auth.data
{
    public interface IApiKey
    {
        string PrimaryApiKey { get; set; }
        string SecondaryApiKey { get; set; }
    }
}