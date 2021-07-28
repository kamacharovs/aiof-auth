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
        private readonly ITenant _tenant;
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly AbstractValidator<TokenRequest> _tokenRequestValidator;

        private readonly string _issuer;
        private readonly string _audience;

        public AuthRepository(
            ILogger<AuthRepository> logger,
            IEnvConfiguration envConfig,
            ITenant tenant,
            IUserRepository userRepo,
            IClientRepository clientRepo,
            AbstractValidator<TokenRequest> tokenRequestValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _envConfig = envConfig ?? throw new ArgumentNullException(nameof(envConfig));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _clientRepo = clientRepo ?? throw new ArgumentNullException(nameof(clientRepo));
            _tokenRequestValidator = tokenRequestValidator ?? throw new ArgumentNullException(nameof(tokenRequestValidator));

            _issuer = _envConfig.JwtIssuer ?? throw new ArgumentNullException(nameof(_envConfig.JwtIssuer));
            _audience = _envConfig.JwtAudience ?? throw new ArgumentNullException(nameof(_envConfig.JwtAudience));
        }

        public async Task<ITokenResponse> GetTokenAsync(ITokenRequest request)
        {
            var tokenRequest = request as TokenRequest;

            switch (request.Type)
            {
                case TokenType.User:
                    await _tokenRequestValidator.ValidateAndThrowAsync(tokenRequest, Constants.EmailPasswordRuleSet);

                    var user = await _userRepo.GetAsync(request.Email, request.Password);
                    var refreshToken = await _userRepo.GetOrAddRefreshTokenAsync(user.Id);

                    return GenerateJwt(user, refreshToken.Token);
                case TokenType.ApiKey:
                    await _tokenRequestValidator.ValidateAndThrowAsync(tokenRequest, Constants.ApiKeyRuleSet);
                    return await GenerateTokenAsync(request.ApiKey);
                case TokenType.Refresh:
                    await _tokenRequestValidator.ValidateAndThrowAsync(tokenRequest, Constants.TokenRuleSet);
                    return await RefreshTokenAsync(request.Token);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request. Please provide the following: a email/password, api_key or refresh_token");
            }
        }

        public async Task<ITokenResponse> GenerateTokenAsync(string apiKey)
        {
            switch (apiKey.DecodeKey())
            {
                case nameof(User):
                    var user = await _userRepo.GetAsync(apiKey);
                    var userRefreshToken = await _userRepo.GetOrAddRefreshTokenAsync(user.Id);
                    return GenerateJwt(
                        user,
                        userRefreshToken.Token);
                case nameof(Client):
                    var client = await _clientRepo.GetAsync(apiKey);
                    
                    if (client.Enabled is false)
                        throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                            $"Client is disabled");

                    var clientRefreshToken = await _clientRepo.GetOrAddRefreshTokenAsync(client);
                    return GenerateJwt(
                        client,
                        clientRefreshToken.Token);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request with ApiKey='{apiKey}'");
            }
        }

        public async Task<ITokenResponse> RefreshTokenAsync(string refreshToken)
        {
            switch (refreshToken.DecodeKey())
            {
                case nameof(User):
                    var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
                    return GenerateJwt(
                        user: user,
                        expiresIn: _envConfig.JwtRefreshExpires);
                case nameof(Client):
                    var client = await _clientRepo.GetByRefreshTokenAsync(refreshToken);
                    return GenerateJwt(
                        client: client,
                        expiresIn: _envConfig.JwtRefreshExpires);
                default:
                    throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                        $"Invalid token request with RefreshToken='{refreshToken}'");
            }
        }

        public ITokenResponse GenerateJwt(
            IUser user,
            string refreshToken = null,
            int? expiresIn = null)
        {
            var token = GenerateJwt<User>(new Claim[]
                {
                    new Claim(AiofClaims.UserId, user.Id.ToString()),
                    new Claim(AiofClaims.PublicKey, user.PublicKey.ToString()),
                    new Claim(AiofClaims.Role, user.Role.Name)
                },
                entity: user as IPublicKeyId,
                refreshToken: refreshToken,
                expiresIn: expiresIn);

            return new TokenResponse
            {
                TokenType = token.TokenType,
                ExpiresIn = token.ExpiresIn,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
        }

        public ITokenResponse GenerateJwt(
            IClient client,
            string refreshToken = null,
            int? expiresIn = null)
        {
            return GenerateJwt<Client>(new Claim[]
                {
                    new Claim(AiofClaims.ClientId, client.Id.ToString()),
                    new Claim(AiofClaims.PublicKey, client.PublicKey.ToString()),
                    new Claim(AiofClaims.Role, client.Role.Name)
                },
                entity: client as IPublicKeyId,
                refreshToken: refreshToken,
                expiresIn: expiresIn);
        }

        public ITokenResponse GenerateJwt<T>(
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
                SigningCredentials = new SigningCredentials(GetRsaKey(RsaKeyType.Private), SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var logJwtMessage = expires == _envConfig.JwtExpires ? "JWT" : "Refresh JWT";
            _logger.LogInformation("Created {JwtMessage} for {EntityName} with Id={EntityId} and PublicKey={EntityPublicKey}",
                logJwtMessage,
                typeof(T).Name,
                entity?.Id,
                entity?.PublicKey);

            return new TokenResponse
            {
                ExpiresIn = expires,
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
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

        public ITokenResult ValidateToken(string token)
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
                    IssuerSigningKey = GetRsaKey(RsaKeyType.Public)
                };

                var handler = new JwtSecurityTokenHandler();
                var result = handler.ValidateToken(
                    token,
                    tokenParams,
                    out var securityToken);

                return new TokenResult
                {
                    IsAuthenticated = result.Identity.IsAuthenticated,
                    Status = TokenStatus.Valid
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
            catch (Exception)
            {
                return new TokenResult
                {
                    IsAuthenticated = false,
                    Status = TokenStatus.Invalid
                };
            }
        }

        public async Task<IRevokeResponse> RevokeTokenAsync(
            string token,
            int? userId = null,
            int? clientId = null)
        {
            if (clientId != null)
            {
                var clientRefresh = await _clientRepo.RevokeTokenAsync((int)clientId, token);

                _logger.LogInformation("Revoked {EntityName} token={EntityToken}",
                    nameof(ClientRefreshToken),
                    clientRefresh.Token);

                return new RevokeResponse
                {
                    Token = clientRefresh.Token,
                    Revoked = clientRefresh.Revoked
                };
            }
            else if (userId != null)
            {
                var userRefresh = await _userRepo.RevokeTokenAsync((int)userId, token);

                _logger.LogInformation("Revoked {EntityName} token={EntityToken}",
                    nameof(UserRefreshToken),
                    userRefresh.Token);

                return new RevokeResponse
                {
                    Token = userRefresh.Token,
                    Revoked = userRefresh.Revoked
                };
            }
            else
                throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Couldn't revoke Token='{token}' for UserId='{userId}' or ClientId='{clientId}'");
        }

        public IIntrospectTokenResult Introspect()
        {
            return new IntrospectTokenResult
            {
                Claims = _tenant.Claims,
                Status = ValidateToken(_tenant.Token).Status
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
