﻿using System;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using FluentValidation;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IEnvConfiguration _envConfig;
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly AbstractValidator<TokenRequest> _tokenRequestValidator;

        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;


        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            IUserRepository userRepo,
            IClientRepository clientRepo,
            AbstractValidator<TokenRequest> tokenRequestValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _clientRepo = clientRepo ?? throw new ArgumentNullException(nameof(clientRepo));
            _tokenRequestValidator = tokenRequestValidator ?? throw new ArgumentNullException(nameof(tokenRequestValidator));

            _key = _envConfig.JwtSecret ?? throw new ArgumentNullException(nameof(_envConfig.JwtSecret));
            _issuer = _envConfig.JwtIssuer ?? throw new ArgumentNullException(nameof(_envConfig.JwtIssuer));
            _audience = _envConfig.JwtAudience ?? throw new ArgumentNullException(nameof(_envConfig.JwtAudience));
        }

        public async Task<ITokenResponse> GetTokenAsync(ITokenRequest request)
        {
            await _tokenRequestValidator.ValidateAndThrowAsync(request as TokenRequest);

            switch (request.Type)
            {
                case TokenType.User:
                    var user = await _userRepo.GetUserAsync(request.Username, request.Password);
                    return GenerateJwtToken(user);
                case TokenType.Client:
                    var clientRefresh = await _clientRepo.AddClientRefreshTokenAsync(request.ApiKey);
                    return GenerateJwtToken(
                        clientRefresh.Client,
                        clientRefresh.Token);
                case TokenType.Refresh:
                    var client = (await _clientRepo.GetRefreshTokenAsync(request.Token)).Client;
                    return RefreshToken(client);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request");
            }
        }

        public async Task<IRevokeResponse> RevokeTokenAsync(
            int clientId,
            string token)
        {
            var clientRefresh = await _clientRepo.RevokeTokenAsync(clientId, token);

            return new RevokeResponse
            {
                ClientId = clientRefresh.ClientId,
                Token = clientRefresh.Token,
                Revoked = clientRefresh.Revoked
            };
        }

        public ITokenResponse RefreshToken(IClient client)
        {
            return GenerateJwtToken(
                client: client,
                expiresIn: _envConfig.JwtRefreshExpires);
        }

        public ITokenResponse GenerateJwtToken(IUser user)
        {
            return GenerateJwtToken<User>(new Claim[]
                {
                    new Claim(AiofClaims.PublicKey, user.PublicKey.ToString()),
                    new Claim(AiofClaims.GivenName, user.FirstName),
                    new Claim(AiofClaims.FamilyName, user.LastName),
                    new Claim(AiofClaims.Email, user.Email)
                },
                entity: user as IPublicKeyId);
        }

        public ITokenResponse GenerateJwtToken(
            IClient client,
            string refreshToken = null,
            int? expiresIn = null)
        {
            return GenerateJwtToken<Client>(new Claim[]
                {
                    new Claim(AiofClaims.PublicKey, client.PublicKey.ToString()),
                    new Claim(AiofClaims.Name, client.Name),
                    new Claim(AiofClaims.Slug, client.Slug)
                },
                entity: client as IPublicKeyId,
                refreshToken: refreshToken,
                expiresIn: expiresIn);
        }

        public ITokenResponse GenerateJwtToken<T>(
            IEnumerable<Claim> claims,
            IPublicKeyId entity = null,
            string refreshToken = null,
            int? expiresIn = null)
            where T : class, IPublicKeyId
        {
            var expires = expiresIn ?? _envConfig.JwtExpires;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(expires),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = GetSigningCredentials()
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"Created JWT for {typeof(T).Name} with Id='{entity?.Id}' and PublicKey='{entity?.PublicKey}'");

            return new TokenResponse
            {
                ExpiresIn = expires,
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };

            /// <summary>
            /// Get <see cref="SigningCredentials"/> based on the type of <typeparamref name="T"/>. 
            /// The credentials are used to sign the JWT (Json Web Token). They are completely configurable on an Entity level; <see cref="User"/>, <see cref="Client"/>, etc.
            /// The default signing credentials use the <see cref="SecurityAlgorithms.RsaSha256"/> algorithm
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns><see cref="SigningCredentials"/></returns>
            SigningCredentials GetSigningCredentials()
            {
                var algType = GetAlgType<T>();

                switch (algType)
                {
                    case AlgType.RS256:
                        return new SigningCredentials(
                            GetRsaKey(RsaKeyType.Private),
                            SecurityAlgorithms.RsaSha256);
                    case AlgType.HS256:
                        var key = Encoding.ASCII.GetBytes(_key);
                        return new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature);
                    default:
                        throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                            $"Invalid or unsupported Alg Type.");
                }
            }
        }

        /// <summary>
        /// Get algorithm <see cref="AlgType"/> of <typeparamref name="T"/> based on the type and environment configuration
        /// </summary>
        /// <remarks>
        /// In a case where a new entity is added, then this part should be the only part that needs to be updated.
        /// The current supported entities are <see cref="User"/> and <see cref="Client"/>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns><see cref="AlgType"/></returns>
        public AlgType GetAlgType<T>()
            where T : class, IPublicKeyId
        {
            string alg = string.Empty;

            switch (typeof(T).Name)
            {
                case nameof(User):
                    alg = _envConfig.JwtAlgorithmUser;
                    break;
                case nameof(Client):
                    alg = _envConfig.JwtAlgorithmClient;
                    break;
                default:
                    alg = _envConfig.JwtAlgorithmDefault;
                    break;
            }

            return alg.ToEnum();
        }

        /// <summary>
        /// Get <see cref="RsaSecurityKey"/> based on <see cref="RsaKeyType"/>. The current values are inheritted certificate signing
        /// via a <see cref="RsaKeyType.Public"/> and <see cref="RsaKeyType.Private"/> keys
        /// </summary>
        /// <param name="rsaKeyType"></param>
        /// <returns><see cref="RsaSecurityKey"/></returns>
        public RsaSecurityKey GetRsaKey(RsaKeyType rsaKeyType)
        {
            var rsa = RSA.Create();

            switch (rsaKeyType)
            {
                case RsaKeyType.Public:
                    rsa.FromXmlString(_envConfig.JwtPublicKey);
                    break;
                case RsaKeyType.Private:
                    rsa.FromXmlString(_envConfig.JwtPrivateKey);
                    break;
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid or unsupported RSA Key Type");
            }

            return new RsaSecurityKey(rsa);
        }

        public ITokenResult ValidateToken<T>(string token)
            where T : class, IPublicKeyId
        {
            try
            {
                var tokenParams = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = _audience,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = GetSecurityKey()
                };

                var handler = new JwtSecurityTokenHandler();
                var result = handler.ValidateToken(
                    token,
                    tokenParams,
                    out var securityToken);

                return new TokenResult
                {
                    IsAuthenticated = result.Identity.IsAuthenticated,
                    Status = TokenResultStatus.Valid.ToString()
                };

                /// <summary>
                /// Get <see cref="SecurityKey"/> based on the type of <typeparamref name="T"/>. 
                /// The key is then used to validate the JWT (Json Web Token) based on the algorithm the JWT (Json Web Token) was signed with
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <returns><see cref="SecurityKey"/></returns>
                SecurityKey GetSecurityKey()
                {
                    var algType = GetAlgType<T>();

                    switch (algType)
                    {
                        case AlgType.RS256:
                            return GetRsaKey(RsaKeyType.Public);
                        case AlgType.HS256:
                            var key = Encoding.ASCII.GetBytes(_key);
                            return new SymmetricSecurityKey(key);
                        default:
                            throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                                $"Invalid or unsupported Alg Type");
                    }
                }
            }
            catch (SecurityTokenExpiredException)
            {
                throw new AuthFriendlyException(HttpStatusCode.Unauthorized,
                    $"Invalid or expired token");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                throw new AuthFriendlyException(HttpStatusCode.Unauthorized,
                    $"Invalid signature");
            }
        }
        public ITokenResult ValidateToken(IValidationRequest request)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(request.AccessToken);

            var givenName = token.Claims.FirstOrDefault(x => x.Type == AiofClaims.GivenName);

            if (!string.IsNullOrWhiteSpace(givenName?.Value))
                return ValidateToken<User>(request.AccessToken);
            else
                return ValidateToken<Client>(request.AccessToken);
        }

        public JsonWebKey GetPublicJsonWebKey()
        {
            var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(GetRsaKey(RsaKeyType.Public));

            jwk.Use = AiofClaims.Sig;
            jwk.Alg = AlgType.RS256.ToString();

            return jwk;
        }

        public IOpenIdConfig GetOpenIdConfig(
            string host,
            bool isHttps)
        {
            var protocol = isHttps ? "https" : "http";

            return new OpenIdConfig
            {
                Issuer = _envConfig.JwtIssuer,
                TokenEndpoint = $"{protocol}://{host}/auth/token",
                TokenRefreshEndpoint = $"{protocol}://{host}/auth/token/refresh",
                JsonWebKeyEndpoint = $"{protocol}://{host}/auth/jwks"
            };
        }
    }
}
