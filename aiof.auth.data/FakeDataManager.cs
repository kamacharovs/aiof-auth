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
                    PublicKey = Guid.NewGuid(),
                    FirstName = "Georgi",
                    LastName = "Kamacharov",
                    Email = "gkama@test.com",
                    Username = "gkama",
                    PrimaryApiKey = "api-key",
                    SecondaryApiKey = "api-key-2",
                    Password = "pass1234"
                },
                new User
                {
                    Id = 2,
                    PublicKey = Guid.NewGuid(),
                    FirstName = "Jessie",
                    LastName = "Brown",
                    Email = "jessie@test.com",
                    Username = "jbro",
                    PrimaryApiKey = "api-key-jbro",
                    SecondaryApiKey = "api-key-jbro-2",
                    Password = "notpass"
                }
            };
        }
    }
}