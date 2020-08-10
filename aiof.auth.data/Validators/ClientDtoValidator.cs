using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        public ClientDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}