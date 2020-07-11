using System;

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

        [Fact]
        public void Test1()
        {

        }
    }
}
