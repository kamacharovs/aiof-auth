using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleSet(Constants.EmailPasswordRuleSet, () => { SetEmailPasswordRuleSet(); });
            RuleSet(Constants.ApiKeyRuleSet, () => { SetApiKeyRuleSet(); });
            RuleSet(Constants.TokenRuleSet, () => { SetTokenRuleSet(); });

            // Either Email, Password is provided, ApiKey (Client or User) is provided or RefreshToken
            //.WithMessage("Invalid token request. Please provide the following: a Email/Password, ApiKey or Token");
        }

        public void SetEmailPasswordRuleSet()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .MaximumLength(200);

            RuleFor(x => x.Password)
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.ApiKey)
                .Null();
                
            RuleFor(x => x.Token)
                .Null();
        }

        public void SetApiKeyRuleSet()
        {
            RuleFor(x => x.Email)
                .Null();

            RuleFor(x => x.Password)
                .Null();

            RuleFor(x => x.ApiKey)
                .NotNull()
                .MaximumLength(100);
           
            RuleFor(x => x.Token)
                .Null();
        }

        public void SetTokenRuleSet()
        {
            RuleFor(x => x.Email)
                .Null();

            RuleFor(x => x.Password)
                .Null();

            RuleFor(x => x.ApiKey)
                .Null();
           
            RuleFor(x => x.Token)
                .NotNull()
                .MaximumLength(200);
        }
    }
}