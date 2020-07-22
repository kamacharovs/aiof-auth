using System;

using FluentValidation;

using Xunit;

using aiof.auth.data;

namespace aiof.auth.tests
{
    public class ValidatorTests
    {
        private readonly AbstractValidator<UserDto> userDtoValidator;

        public ValidatorTests()
        {
            userDtoValidator = Helper.GetRequiredService<AbstractValidator<UserDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<UserDto>));
        }
    }
}