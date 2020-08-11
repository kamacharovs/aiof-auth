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
                case TokenRequestType.User:
                    var user = await _userRepo.GetUserAsync(request.Username, request.Password);
                    return GenerateJwtToken(user);
                case TokenRequestType.Client:
                    var clientRefresh = await _clientRepo.AddClientRefreshTokenAsync(request.ApiKey);
                    return GenerateJwtToken(
                        clientRefresh.Client,
                        clientRefresh.Token);
                case TokenRequestType.Refresh:
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
                });
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
                refreshToken,
                expiresIn);
        }

        public ITokenResponse GenerateJwtToken<T>(
            IEnumerable<Claim> claims,
            string refreshToken = null,
            int? expiresIn = null)
            where T : class, IPublicKeyId
        {
            var expires = expiresIn ?? _envConfig.JwtExpires;
            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenHandler = new JwtSecurityTokenHandler();

            claimsIdentity.AddClaim(new Claim(AiofClaims.Entity, typeof(T).Name));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddSeconds(expires),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = GetSigningCredentials<T>()
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation($"Created JWT for {typeof(T).Name}");// with Id='{T.Id}' and PublicKey='{T.PublicKey}'");

            return new TokenResponse
            {
                ExpiresIn = expires,
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        public SigningCredentials GetSigningCredentials<T>()    //NEW
            where T : class, IPublicKeyId
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

        public SecurityKey GetSecurityKey<T>()  //NEW
            where T : class, IPublicKeyId
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

        public AlgType GetAlgType<T>()    //NEW
            where T : class, IPublicKeyId
        {
            switch (typeof(T).Name)
            {
                case nameof(User):
                    return _envConfig.JwtAlgorithmUser.ToEnum();
                case nameof(Client):
                    return _envConfig.JwtAlgorithmClient.ToEnum();
                default:
                    return _envConfig.JwtAlgorithmDefault.ToEnum();
            }
        }

        public RsaSecurityKey GetRsaKey(RsaKeyType rsaKeyType)    //NEW
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
                    IssuerSigningKey = GetSecurityKey<T>()
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

            var entity = token.Claims.FirstOrDefault(x => x.Type == AiofClaims.Entity).Value;

            if (entity == nameof(User))
                return ValidateToken<User>(request.AccessToken);
            else if (entity == nameof(Client))
                return ValidateToken<Client>(request.AccessToken);
            else
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Invalid Entity type");
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
