using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            // Either Username, Password is provided, ApiKey is provided or RefreshToken
            RuleFor(x => x)
                .Must(x => 
                {
                    if (!string.IsNullOrWhiteSpace(x.Username)
                        && !string.IsNullOrWhiteSpace(x.Password)
                        && string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.ApiKey))
                    {
                        x.Type = TokenType.User;
                        return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(x.ApiKey)
                        && string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.Username)
                        && string.IsNullOrWhiteSpace(x.Password))
                    {
                        x.Type = TokenType.Client;
                        return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.Username)
                        && string.IsNullOrWhiteSpace(x.Password)
                        && string.IsNullOrWhiteSpace(x.ApiKey))
                    {
                        x.Type = TokenType.Refresh;
                        return true;
                    }

                    return false;
                })
                .WithMessage("Invalid token request. Please provide the following: a Username/Password, ApiKey or Token");
        }
    }
}