using System;
using System.Threading.Tasks;

using Xunit;

using aiof.auth.services;

namespace aiof.auth.tests
{
    public class AuthRepositoryTests
    {
        private readonly IAuthRepository _repo;

        public AuthRepositoryTests()
        {
            _repo = Helper.GetRequiredService<IAuthRepository>() ?? throw new ArgumentNullException(nameof(IAuthRepository));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetUserTokenAsync_Valid(int id)
        {
            var userToken = await _repo.GetUserTokenAsync(id);

            Assert.NotNull(userToken);
        }
    }
}
