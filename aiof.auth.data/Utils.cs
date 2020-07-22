using System;
using System.Security.Cryptography;

namespace aiof.auth.data
{
    public static class Utils
    {
        public static string GenerateApiKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}