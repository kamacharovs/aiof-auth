using System;
using System.Linq;

using FluentValidation;

using Xunit;

using aiof.auth.data;

namespace aiof.auth.tests
{
    public class ValidatorTests
    {
        private readonly AbstractValidator<UserDto> _userDtoValidator;

        public ValidatorTests()
        {
            _userDtoValidator = Helper.GetRequiredService<AbstractValidator<UserDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<UserDto>));
        }

        [Theory]
        [InlineData("Password1234")]
        [InlineData("Testing@92")]
        public void UserDto_ValidatePassword_Valid(string password)
        {
            var userDto = Helper.RandomUserDtos(1).First();

            userDto.Password = password;

            Assert.True(_userDtoValidator.Validate(userDto).IsValid);
        }

        [Theory]
        [InlineData("password1234")]
        [InlineData("")]
        public void UserDto_ValidatePassword_InValid(string password)
        {
            var userDto = Helper.RandomUserDtos(1).First();

            userDto.Password = password;

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }
    }
}