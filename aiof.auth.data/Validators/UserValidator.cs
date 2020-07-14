using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName)
                .NotEmpty();
            RuleFor(x => x.LastName)
                .NotEmpty();
            RuleFor(x => x.Email)
                .NotEmpty();
            RuleFor(x => x.Username)
                .NotEmpty();
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}