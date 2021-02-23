using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aiof.auth.data
{
    public static class Constants
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationProblemJson = "application/problem+json";
    }

    public enum AccessTokenStatus
    {
        Valid,
        Invalid,
        Expired
    }
}
