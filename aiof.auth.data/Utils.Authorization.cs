using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using JetBrains.Annotations;

namespace aiof.auth.data
{
    public static partial class Utils
    {
        public static RsaSecurityKey GetRsaKey(
            [NotNull] this IEnvConfiguration envConfig,
            RsaKeyType rsaKeyType)
        {
            var rsa = RSA.Create();

            switch (rsaKeyType)
            {
                case RsaKeyType.Public:
                    rsa.FromXmlString(envConfig.JwtPublicKey);
                    break;
                case RsaKeyType.Private:
                    rsa.FromXmlString(envConfig.JwtPrivateKey);
                    break;
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid or unsupported RSA Key Type");
            }

            return new RsaSecurityKey(rsa);
        }

        public static AlgType GetAlgType<T>(
            [NotNull] this IEnvConfiguration envConfig)
            where T : class, IPublicKeyId
        {
            string alg = string.Empty;

            switch (typeof(T).Name)
            {
                case nameof(User):
                    alg = envConfig.JwtAlgorithmUser;
                    break;
                case nameof(Client):
                    alg = envConfig.JwtAlgorithmClient;
                    break;
                default:
                    alg = envConfig.JwtAlgorithmDefault;
                    break;
            }

            return alg.ToEnum();
        }

        public static SigningCredentials GetSigningCredentials<T>(
            [NotNull] this IEnvConfiguration envConfig)
            where T : class, IPublicKeyId
        {
            var algType = envConfig.GetAlgType<T>();

            switch (algType)
            {
                case AlgType.RS256:
                    return new SigningCredentials(
                        envConfig.GetRsaKey(RsaKeyType.Private),
                        SecurityAlgorithms.RsaSha256);
                case AlgType.HS256:
                    var key = Encoding.ASCII.GetBytes(envConfig.JwtSecret);
                    return new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid or unsupported Alg Type.");
            }
        }

        public static SecurityKey GetSecurityKey<T>(
            [NotNull] this IEnvConfiguration envConfig)
            where T : class, IPublicKeyId
        {
            var algType = envConfig.GetAlgType<T>();

            switch (algType)
            {
                case AlgType.RS256:
                    return envConfig.GetRsaKey(RsaKeyType.Public);
                case AlgType.HS256:
                    var key = Encoding.ASCII.GetBytes(envConfig.JwtSecret);
                    return new SymmetricSecurityKey(key);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid or unsupported Alg Type");
            }
        }
    }
}
