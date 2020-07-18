using System;
using System.Linq;
using System.Collections.Generic;

using Bogus;

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

            _context.Clients
                .AddRange(GetFakeClients());

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
                    Password = "10000.JiFzc3Ijb5vBrCb8COiNzA==.BzdHomm3RMu0sMHaBfTpY0B2WtbjFqi9tN7T//N+khA=" //pass1234
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
                    Password = "10000.nBfnY+XzDhvP7Z2RcTLTtA==.rj6rCGGLRz5bvTxZj+cB8X+GbYf1nTu0x9iW2v3wEYc=" //password123
                }
            };
        }

        public IEnumerable<Client> GetFakeClients()
        {
            return new List<Client>
            {
                new Client
                {
                    Id = 1,
                    PublicKey = Guid.Parse("306a90cc-7d55-4829-8da0-55f8f047addd"),
                    Name = "GK Client 1",
                    Slug = "gk-client-1",
                    Enabled = true,
                    PrimaryApiKey = "gk-client-1-p-key",
                    SecondaryApiKey = "gk-client-1-s-key"
                },
                new Client
                {
                    Id = 2,
                    PublicKey = Guid.Parse("517dd6b8-8dc1-49e0-8e4d-2759379a8bc8"),
                    Name = "GK Client 2",
                    Slug = "gk-client-2",
                    Enabled = true,
                    PrimaryApiKey = "gk-client-2-p-key",
                    SecondaryApiKey = "gk-client-2-s-key"
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
                    Name = AiofClaims.GivenName
                },
                new AiofClaim
                {
                    Id = 2,
                    Name = AiofClaims.FamilyName
                },
                new AiofClaim
                {
                    Id = 3,
                    Name = AiofClaims.Name
                },
                new AiofClaim
                {
                    Id = 4,
                    Name = AiofClaims.Email
                }
            };
        }

        private User GetFakeUser()
        {
            return new Faker<User>()
                .RuleFor(x => x.Id, f => f.Random.Int(0, 100))
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .Generate();
        }

        public IEnumerable<object[]> GetFakeUsersData(
            bool id = false,
            bool apiKey = false,
            bool username = false,
            bool password = false
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
            else if (username && password)
                return new List<object[]>
                {
                    new object[] { fakeUsers[0].Username, "pass1234" },
                    new object[] { fakeUsers[1].Username, "password123" }
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

        public IEnumerable<object[]> GetFakeClientsData(
            bool id = false,
            bool apiKey = false
        )
        {
            var fakeClients = GetFakeClients()
                .ToArray();

            if (id && apiKey)
                return new List<object[]>
                {
                    new object[] { fakeClients[0].Id, fakeClients[0].PrimaryApiKey },
                    new object[] { fakeClients[1].Id, fakeClients[1].PrimaryApiKey }
                };
            else if (id)
                return new List<object[]>
                {
                    new object[] { fakeClients[0].Id },
                    new object[] { fakeClients[1].Id }
                };
            else if (apiKey)
                return new List<object[]>
                {
                    new object[] { fakeClients[0].PrimaryApiKey },
                    new object[] { fakeClients[1].PrimaryApiKey }
                }; 
            else
                return null;
        }

        public IEnumerable<object[]> GetFakeClaimsData()
        {
            var fakeClaims = GetFakeClaims()
                .ToArray();

            var claims = new List<object[]>();

            for (int i = 0; i < fakeClaims.Count(); i++)
                claims.Add(new object[]
                    { 
                        fakeClaims[i].Id,
                        fakeClaims[i].PublicKey, 
                        fakeClaims[i].Name 
                    });
            
            return claims;
        }

        public string ExpiredJwtToken => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdWJsaWNfa2V5IjoiNTgxZjNjZTYtY2YyYS00MmE1LTgyOGYtMTU3YTJiZmFiNzYzIiwiZ2l2ZW5fbmFtZSI6Ikdlb3JnaSIsImZhbWlseV9uYW1lIjoiS2FtYWNoYXJvdiIsImVtYWlsIjoiZ2thbWFAdGVzdC5jb20iLCJuYmYiOjE1OTQ4MzY5NTksImV4cCI6MTU5NDgzNzg1OCwiaWF0IjoxNTk0ODM2OTU5LCJpc3MiOiJhaW9mOmF1dGgiLCJhdWQiOiJhaW9mOmF1dGg6YXVkaWVuY2UifQ.RYiY7lr7uVb6gBHovsV4qlpHgqa7WEmq-Uhd_8F7B1o";
    }
}