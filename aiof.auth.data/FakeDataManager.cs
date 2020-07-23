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

            _context.ClientRefreshTokens
                .AddRange(GetFakeClientRefreshTokens());

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
                    Password = "10000.nBfnY+XzDhvP7Z2RcTLTtA==.rj6rCGGLRz5bvTxZj+cB8X+GbYf1nTu0x9iW2v3wEYc=" //password123
                },
                GetRandomFakeUser()
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
                },
                GetRandomFakeClient()
            };
        }

        public IEnumerable<ClientRefreshToken> GetFakeClientRefreshTokens()
        {
            return new List<ClientRefreshToken>
            {
                new ClientRefreshToken
                {
                    Id = 1,
                    PublicKey = Guid.Parse("239eebf7-30f1-4f32-b1f1-18622dc2342d"),
                    Token = "refresh-token-1",
                    ClientId = 1
                },
                new ClientRefreshToken
                {
                    Id = 2,
                    PublicKey = Guid.Parse("c8f80b28-3459-42b8-9c13-30e719a14df7"),
                    Token = "refresh-token-2",
                    ClientId = 2,
                    Created = DateTime.UtcNow.AddDays(-2)
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

        public User GetRandomFakeUser()
        {
            return new Faker<User>()
                .RuleFor(x => x.Id, f => f.Random.Int(5, 20))
                .RuleFor(x => x.PublicKey, f => Guid.NewGuid())
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Username, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(x => x.Password, f => HashedPassword)
                .Generate();
        }
        public IEnumerable<UserDto> GetRandomFakeUserDtos(int n)
        {
            return new Faker<UserDto>()
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Username, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(x => x.Password, f => HashedPassword)
                .Generate(n);
        }

        public Client GetRandomFakeClient()
        {
            return new Faker<Client>()
                .RuleFor(x => x.Id, f => f.Random.Int(5, 20))
                .RuleFor(x => x.PublicKey, f => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Slug, f => f.Random.String())
                .RuleFor(x => x.Enabled, f => true)
                .RuleFor(x => x.PrimaryApiKey, f => f.Random.String())
                .RuleFor(x => x.SecondaryApiKey, f => f.Random.String())
                .Generate();
        }
        public IEnumerable<ClientDto> GetRandomFakeClientDtos(int n)
        {
            return new Faker<ClientDto>()
                .RuleFor(x => x.Name, f => f.Random.String())
                .RuleFor(x => x.Slug, f => f.Internet.DomainName().ToLower())
                .RuleFor(x => x.Enabled, f => true)
                .Generate(n);
        }

        public IEnumerable<object[]> GetFakeUsersData(
            bool id = false,
            bool username = false,
            bool password = false
        )
        {
            var fakeUsers = GetFakeUsers()
                .ToArray();

            if (username && password)
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
            else
                return null;
        }

        public IEnumerable<object[]> GetFakeUserDtosData()
        {
            var fakeUserDtos = GetRandomFakeUserDtos(3);
            var fakeUserDtosList = new List<object[]>();

            foreach (var fakeUserDto in fakeUserDtos)
            {
                fakeUserDtosList.Add(new object[]
                {
                    fakeUserDto.FirstName,
                    fakeUserDto.LastName,
                    fakeUserDto.Email,
                    fakeUserDto.Username,
                    fakeUserDto.Password
                });
            }

            return fakeUserDtosList;
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

        public IEnumerable<object[]> GetFakeClientsDtoData()
        {
            var clientDtos = GetRandomFakeClientDtos(3)
                .ToArray();

            return new List<object[]>
            {
                new object[] { clientDtos[0].Name, clientDtos[0].Slug, clientDtos[0].Enabled },
                new object[] { clientDtos[1].Name, clientDtos[1].Slug, clientDtos[1].Enabled },
                new object[] { clientDtos[2].Name, clientDtos[2].Slug, clientDtos[2].Enabled }
            };
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

        public string HashedPassword => "10000.JiFzc3Ijb5vBrCb8COiNzA==.BzdHomm3RMu0sMHaBfTpY0B2WtbjFqi9tN7T//N+khA="; //pass1234
        public string ExpiredJwtToken => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdWJsaWNfa2V5IjoiNTgxZjNjZTYtY2YyYS00MmE1LTgyOGYtMTU3YTJiZmFiNzYzIiwiZ2l2ZW5fbmFtZSI6Ikdlb3JnaSIsImZhbWlseV9uYW1lIjoiS2FtYWNoYXJvdiIsImVtYWlsIjoiZ2thbWFAdGVzdC5jb20iLCJuYmYiOjE1OTQ4MzY5NTksImV4cCI6MTU5NDgzNzg1OCwiaWF0IjoxNTk0ODM2OTU5LCJpc3MiOiJhaW9mOmF1dGgiLCJhdWQiOiJhaW9mOmF1dGg6YXVkaWVuY2UifQ.RYiY7lr7uVb6gBHovsV4qlpHgqa7WEmq-Uhd_8F7B1o";
    }
}