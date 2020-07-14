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

            _context.Claims
                .AddRange(GetFakeClaims());

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

        public IEnumerable<AiofClaim> GetFakeClaims()
        {
            return new List<AiofClaim>
            {
                new AiofClaim
                {
                    Id = 1,
                    PublicKey = Guid.NewGuid(),
                    Name = "first_name"
                },
                new AiofClaim
                {
                    Id = 2,
                    PublicKey = Guid.NewGuid(),
                    Name = "last_name"
                },
                new AiofClaim
                {
                    Id = 3,
                    PublicKey = Guid.NewGuid(),
                    Name = "full_name"
                },
                new AiofClaim
                {
                    Id = 4,
                    PublicKey = Guid.NewGuid(),
                    Name = "email"
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

        public IEnumerable<object[]> GetFakeClaimsData()
        {
            var fakeClaims = GetFakeClaims()
                .ToArray();

            return new List<object[]>
            {
                new object[] { fakeClaims[0].Id, fakeClaims[0].PublicKey, fakeClaims[0].Name },
                new object[] { fakeClaims[1].Id, fakeClaims[1].PublicKey, fakeClaims[1].Name },
                new object[] { fakeClaims[2].Id, fakeClaims[2].PublicKey, fakeClaims[2].Name }
            };
        }
    }
}