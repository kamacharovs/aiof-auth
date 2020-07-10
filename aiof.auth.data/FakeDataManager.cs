using System;
using System.Collections.Generic;

namespace aiof.auth.data
{
    public class FakeDataManager
    {
        private readonly AuthContext _context;

        public FakeDataManager(AuthContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void UseFakeContext()
        {
            _context.Users
                .AddRange(GetFakeUsers());

            _context.SaveChanges();
        }

        public IEnumerable<User> GetFakeUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Georgi",
                    LastName = "Kamacharov",
                    Email = "test@test.com",
                    Username = "gkama",
                    ApiKey = "api-key",
                    Password = "pass1234"
                }
            };
        }
    }
}