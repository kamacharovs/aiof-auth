using System;
using System.Linq;

using FluentValidation;

using Xunit;

using aiof.auth.data;

namespace aiof.auth.tests
{
    [Trait(Helper.Category, Helper.UnitTest)]
    public class ValidatorTests
    {
        private readonly AbstractValidator<UserDto> _userDtoValidator;
        private readonly AbstractValidator<ClientDto> _clientDtoValidator;
        private readonly AbstractValidator<TokenRequest> _tokenRequestValidator;
        private readonly AbstractValidator<AiofClaim> _claimValidator;

        public ValidatorTests()
        {
            var serviceHelper = new ServiceHelper();

            _userDtoValidator = serviceHelper.GetRequiredService<AbstractValidator<UserDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<UserDto>));
            _clientDtoValidator = serviceHelper.GetRequiredService<AbstractValidator<ClientDto>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<ClientDto>));
            _tokenRequestValidator = serviceHelper.GetRequiredService<AbstractValidator<TokenRequest>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<TokenRequest>));
            _claimValidator = serviceHelper.GetRequiredService<AbstractValidator<AiofClaim>>() ?? throw new ArgumentNullException(nameof(AbstractValidator<AiofClaim>));
        }

        [Theory]
        [InlineData("Password1234")]
        [InlineData("Testing@92")]
        [InlineData("thisISALSOvalid@1")]
        public void UserDto_Validate_Password_IsSuccessful(string password)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.Password = password;

            Assert.True(_userDtoValidator.Validate(userDto).IsValid);
        }
        [Theory]
        [InlineData("short")]
        [InlineData("password1234")]
        [InlineData("")]
        [InlineData(null)]
        public void UserDto_Validate_Password_Fails(string password)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.Password = password;

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }

        [Theory]
        [InlineData("Georgi")]
        [InlineData("John")]
        [InlineData("Jessie")]
        [InlineData("Jeff")]
        public void UserDto_Validate_FirstName_TooLong_Fails(string firstName)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.FirstName = firstName.Repeat(100);

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }
        [Theory]
        [InlineData("Kamacharov")]
        [InlineData("Doe")]
        [InlineData("Brown")]
        [InlineData("Bezos")]
        public void UserDto_Validate_LastName_TooLong_Fails(string lastName)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.LastName = lastName.Repeat(100);

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }

        [Theory]
        [InlineData("gkama@aiof.com")]
        [InlineData("john.doe@yahoo.com")]
        [InlineData("jeff.bezos@amazon.com")]
        [InlineData("test@fb.com")]
        public void UserDto_Validate_EmailAddress_IsSuccessful(string email)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.Email = email;

            Assert.True(_userDtoValidator.Validate(userDto).IsValid);
        }
        [Theory]
        [InlineData("gkama")]
        [InlineData("john.doe.com")]
        [InlineData("definitelynotanemail")]
        [InlineData("")]
        public void UserDto_Validate_EmailAddress_Fails(string email)
        {
            var userDto = Helper.FakerUserDtos().First();

            userDto.Email = email;

            Assert.False(_userDtoValidator.Validate(userDto).IsValid);
        }

        [Theory]
        [InlineData("test client")]
        [InlineData("Client 1")]
        public void ClientDto_Validate_Valid(string name)
        {
            var clientDto = new ClientDto
            {
                Name = name
            };

            Assert.True(_clientDtoValidator.Validate(clientDto).IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ClientDto_Validate_Invalid(string name)
        {
            var clientDto = new ClientDto
            {
                Name = name
            };

            Assert.False(_clientDtoValidator.Validate(clientDto).IsValid);
        }

        [Theory]
        [InlineData("test@aiof.com", "test", null)]
        [InlineData(null, null, "test")]
        public void TokenRequest_Validate_IsSuccessful(
            string email, 
            string password, 
            string apiKey)
        {
            var validation = _tokenRequestValidator
                .Validate(new TokenRequest
                {
                    Email = email,
                    Password = password,
                    ApiKey = apiKey
                });

            Assert.True(validation.IsValid);
        }

        [Theory]
        [InlineData("email@aiof.com", "pass", "apikey")]
        [InlineData("email@aiof.com", null, null)]
        [InlineData(null, "pass", null)]
        [InlineData("email@aiof.com", null, "apikey")]
        [InlineData(null, "pass", "apikey")]
        [InlineData(null, null, null)]
        public void TokenRequest_Validate_IsInvalid(
            string email, 
            string password, 
            string apiKey)
        {
            var validation = _tokenRequestValidator
                .Validate(new TokenRequest
                {
                    Email = email,
                    Password = password,
                    ApiKey = apiKey
                });

            Assert.False(validation.IsValid);
        }

        [Theory]
        [InlineData("public_key")]
        [InlineData("given_name")]
        [InlineData("family_name")]
        public void AiofClaim_Validate_Valid(string name)
        {
            var claim = new AiofClaim
            {
                Name = name
            };

            Assert.True(_claimValidator.Validate(claim).IsValid);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("1")]
        [InlineData("")]
        [InlineData(null)]
        public void AiofClaim_Validate_Invalid(string name)
        {
            var claim = new AiofClaim
            {
                Name = name
            };

            Assert.False(_claimValidator.Validate(claim).IsValid);
        }
    }
}