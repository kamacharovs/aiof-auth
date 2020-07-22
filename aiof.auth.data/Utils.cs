using System;
using System.Security.Cryptography;

namespace aiof.auth.data
{
    public static class Utils
    {
        public static string GenerateApiKey(int length = 32)
        {
            var key = new byte[length];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}