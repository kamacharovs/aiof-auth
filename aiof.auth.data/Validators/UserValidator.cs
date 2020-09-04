using System;

using FluentValidation;

namespace aiof.auth.data
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.PublicKey)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200)
                .NotEqual(x => x.LastName);

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200)
                .NotEqual(x => x.FirstName);

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.Username)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            When(x => !string.IsNullOrEmpty(x.PrimaryApiKey), () =>
            {
                RuleFor(x => x.PrimaryApiKey)
                    .Must(x =>
                    {
                        return x.DecodeKey() == nameof(User)
                            ? true
                            : false;
                    });
            });
            When(x => !string.IsNullOrEmpty(x.SecondaryApiKey), () =>
            {
                RuleFor(x => x.SecondaryApiKey)
                    .Must(x =>
                    {
                        return x.DecodeKey() == nameof(User)
                            ? true
                            : false;
                    });
            });
        }
    }
}