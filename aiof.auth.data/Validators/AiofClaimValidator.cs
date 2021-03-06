using System;
using System.Linq;

using FluentValidation;

namespace aiof.auth.data
{
    public class AiofClaimValidator : AbstractValidator<AiofClaim>
    {
        public AiofClaimValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Name)
                .Must(x => AiofClaims.All.Contains(x))
                .WithMessage("Invalid Claim. Valid Claims are: " + string.Join(", ", AiofClaims.All));
        }
    }
}