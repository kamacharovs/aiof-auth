using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

            // Either Username, Password is provided or ApiKey
            RuleFor(x => x)
                .Must(x => 
                {
                    if (!string.IsNullOrWhiteSpace(x.Username)
                        && !string.IsNullOrWhiteSpace(x.Password)
                        && string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.ApiKey))
                    {
                        x.Type = TokenRequestType.User;
                        return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(x.ApiKey)
                        && string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.Username)
                        && string.IsNullOrWhiteSpace(x.Password))
                    {
                        x.Type = TokenRequestType.Client;
                        return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(x.Token)
                        && string.IsNullOrWhiteSpace(x.Username)
                        && string.IsNullOrWhiteSpace(x.Password)
                        && string.IsNullOrWhiteSpace(x.ApiKey))
                    {
                        x.Type = TokenRequestType.ClientWithRefresh;
                        return true;
                    }

                    return false;
                });
        }
    }
}