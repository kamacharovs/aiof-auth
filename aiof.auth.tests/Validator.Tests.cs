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
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;
        private readonly AbstractValidator<TokenRequest> _tokenRequestValidator;

        public ValidatorTests()
        {
            _userDtoValidator = Helper.GetRequiredService<AbstractValidator<UserDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<UserDto>));
            _clientDtoValidator = Helper.GetRequiredService<AbstractValidator<ClientDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<ClientDto>));
            _tokenRequestValidator = Helper.GetRequiredService<AbstractValidator<TokenRequest>>()  ?? throw new ArgumentNullException(nameof(AbstractValidator<TokenRequest>));
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
        public void UserDto_ValidatePassword_Invalid(string password)
        {
            var userDto = Helper.RandomUserDtos(1).First();

            userDto.Password = password;

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }

        [Theory]
        [InlineData("test client", "test-client-slug")]
        [InlineData("Client 1", "client-1-slug")]
        public void ClientDto_Validate_Valid(string name, string slug)
        {
            var clientDto = new ClientDto
            {
                Name = name,
                Slug = slug
            };

            Assert.True(_clientDtoValidator.Validate(clientDto).IsValid);
        }

        [Theory]
        [InlineData("", "test-client-slug")]
        [InlineData(null, "test-client-slug")]
        [InlineData("Client 1", "")]
        [InlineData("Client 1", null)]
        public void ClientDto_Validate_Invalid(string name, string slug)
        {
            var clientDto = new ClientDto
            {
                Name = name,
                Slug = slug
            };

            Assert.False(_clientDtoValidator.Validate(clientDto).IsValid);
        }

        [Theory]
        [InlineData("test", "test", null)]
        [InlineData(null, null, "test")]
        public void TokenRequest_Valid(string username, string password, string apiKey)
        {
            var validation = _tokenRequestValidator
                .Validate(new TokenRequest
                {
                    Username = username,
                    Password = password,
                    ApiKey = apiKey
                });

            Assert.True(validation.IsValid);
        }

        [Theory]
        [InlineData("username", "pass", "apikey")]
        [InlineData("username", null, null)]
        [InlineData(null, "pass", null)]
        [InlineData("username", null, "apikey")]
        [InlineData(null, "pass", "apikey")]
        [InlineData(null, null, null)]
        public void TokenRequest_Invalid(string username, string password, string apiKey)
        {
            var validation = _tokenRequestValidator
                .Validate(new TokenRequest
                {
                    Username = username,
                    Password = password,
                    ApiKey = apiKey
                });

            Assert.False(validation.IsValid);
        }
    }
}