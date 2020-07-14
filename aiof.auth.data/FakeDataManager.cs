using System;
using System.Linq;
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
                    PublicKey = Guid.Parse("581f3ce6-cf2a-42a5-828f-157a2bfab763"),
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
                    PublicKey = Guid.Parse("8e17276c-88ac-43bd-a9e8-5fdf5381dbd5"),
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

        public IEnumerable<object[]> GetFakeUsersData(
            bool id = false,
            bool apiKey = false
        )
        {
            var fakeUsers = GetFakeUsers()
                .ToArray();

            if (id && apiKey)
                return new List<object[]>
                {
                    new object[] { fakeUsers[0].Id, fakeUsers[0].PrimaryApiKey },
                    new object[] { fakeUsers[1].Id, fakeUsers[1].PrimaryApiKey }
                };
            else if (id)
                return new List<object[]>
                {
                    new object[] { fakeUsers[0].Id },
                    new object[] { fakeUsers[1].Id }
                };
            else if (apiKey)
                return new List<object[]>
                {
                    new object[] { fakeUsers[0].PrimaryApiKey },
                    new object[] { fakeUsers[1].PrimaryApiKey }
                }; 
            else
                return null;
        }
    }
}