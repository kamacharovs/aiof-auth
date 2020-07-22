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
            RuleFor(x => new { x.Username, x.Password, x.ApiKey })
                .Must(x => 
                {
                    if (!string.IsNullOrWhiteSpace(x.Username)
                        && !string.IsNullOrWhiteSpace(x.Password)
                        && string.IsNullOrWhiteSpace(x.ApiKey))
                    {
                        return true;
                    }
                    else if (!string.IsNullOrWhiteSpace(x.ApiKey)
                        && string.IsNullOrWhiteSpace(x.Username)
                        && string.IsNullOrWhiteSpace(x.Password))
                    {
                        return true;
                    }

                    return false;
                });
        }
    }
}