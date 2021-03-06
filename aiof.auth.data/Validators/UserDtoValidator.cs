using System;
using System.Text.RegularExpressions;

using FluentValidation;

namespace aiof.auth.data
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            ValidatorOptions.Global.CascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotNull();

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100)
                .Must(x => IsValid(x))
                .WithMessage("Password must meet the following requirements: has a number, has an upper character, has between 8 and 50 characters");
        }

        public bool IsValid(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Maximum50Chars = new Regex(@".{8,50}");

            return hasNumber.IsMatch(password) 
                && hasUpperChar.IsMatch(password) 
                && hasMinimum8Maximum50Chars.IsMatch(password);
        }
    }
}