using System;
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
                case TokenType.ApiKey:
                    return await GenerateJwtTokenAsync(request.ApiKey);
                case TokenType.Refresh:
                    var client = (await _clientRepo.GetRefreshTokenAsync(request.Token)).Client;
                    return RefreshToken(client);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request");
            }
        }
     
        public ITokenResponse RefreshToken(IClient client)
        {
            return GenerateJwtToken(
                client: client,
                expiresIn: _envConfig.JwtRefreshExpires);
        }

        public async Task<ITokenResponse> GenerateJwtTokenAsync(string apiKey)
        {
            switch (apiKey.DecodeApiKey())
            {
                case nameof(User):
                    var user = await _userRepo.GetUserAsync(apiKey);
                    return GenerateJwtToken(user);
                case nameof(Client):
                    var clientRefresh = await _clientRepo.AddClientRefreshTokenAsync(apiKey);
                    return GenerateJwtToken(
                        clientRefresh.Client,
                        clientRefresh.Token);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request with ApiKey='{apiKey}'");
            }
        }

        public ITokenUserResponse GenerateJwtToken(IUser user)
        {
            var token = GenerateJwtToken<User>(new Claim[]
                {
                    new Claim(AiofClaims.PublicKey, user.PublicKey.ToString()),
                    new Claim(AiofClaims.GivenName, user.FirstName),
                    new Claim(AiofClaims.FamilyName, user.LastName),
                    new Claim(AiofClaims.Email, user.Email)
                },
                entity: user as IPublicKeyId);

            return new TokenUserResponse
            {
                TokenType = token.TokenType,
                ExpiresIn = token.ExpiresIn,
                AccessToken = token.AccessToken,
                User = user as User
            };
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
                    Status = TokenResultStatus.Valid.ToString(),
                    EntityType = typeof(T).Name
                };
                
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
