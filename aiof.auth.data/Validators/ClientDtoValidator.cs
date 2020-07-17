using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class ClientDtoValidator : AbstractValidator<ClientDto>
    {
        public ClientDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Slug)
                .NotNull()
                .NotEmpty();
        }
    }
}