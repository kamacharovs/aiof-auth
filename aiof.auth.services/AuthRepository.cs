using System;
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
            var validation = _tokenRequestValidator.Validate(request as TokenRequest);

            if (!validation.IsValid)
                throw new AuthValidationException(validation.Errors);

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
            return GenerateJwtToken(new Claim[]
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
            return GenerateJwtToken(new Claim[]
                {
                    new Claim(AiofClaims.PublicKey, client.PublicKey.ToString()),
                    new Claim(AiofClaims.Name, client.Name),
                    new Claim(AiofClaims.Slug, client.Slug)
                },
                client as IPublicKeyId,
                refreshToken,
                expiresIn);
        }

        public ITokenResponse GenerateJwtToken(
            IEnumerable<Claim> claims, 
            IPublicKeyId entity = null,
            string refreshToken = null, 
            int? expiresIn = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);
            var expires = expiresIn ?? _envConfig.JwtExpires;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(expires),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (entity != null)
                _logger.LogInformation($"Created JWT for {entity.GetType().Name} with Id='{entity.Id}' and PublicKey='{entity.PublicKey}'");

            return new TokenResponse
            {
                ExpiresIn = expires,
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        public ITokenResult ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_key);
                var tokenParams = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = _audience,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
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
        }
        public ITokenResult ValidateToken(IValidationRequest request)
        {
            return ValidateToken(request.AccessToken);
        }

        public JsonWebKey GetPublicJsonWebKey()
        {
            var key = Encoding.ASCII.GetBytes(_key);
            var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(new SymmetricSecurityKey(key));

            jwk.Use = AiofClaims.Sig;
            jwk.Alg = "RS256";

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
