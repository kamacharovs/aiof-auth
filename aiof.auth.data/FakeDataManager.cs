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

            _context.Clients
                .AddRange(GetFakeClients());

            _context.UserRefreshTokens
                .AddRange(GetFakeUserRefreshTokens());

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
                new User
                {
                    Id = 3,
                    PublicKey = Guid.Parse("7c135230-2889-4cbb-bb0e-ab4237d89367"),
                    FirstName = "George",
                    LastName = "Best",
                    Email = "george.best@auth.com",
                    Username = "gbest",
                    Password = "10000.JiFzc3Ijb5vBrCb8COiNzA==.BzdHomm3RMu0sMHaBfTpY0B2WtbjFqi9tN7T//N+khA=", //pass1234
                    PrimaryApiKey = "VXNlcg==.x0sHnNHFytELB6FkLb/L6Q/YXPoXAZ4bAHvztgr6vIU=",
                    SecondaryApiKey = "VXNlcg==.VmNKkE4o6zxCq8Ut1BzkSU2R7RcCqo8y/jTklcTU6m8="
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
                    PrimaryApiKey = "Q2xpZW50.YJj7MeyO1P9DpglkO8bFeAe6vYEBrFhpC9O6BrYR43w=",
                    SecondaryApiKey = "Q2xpZW50.k3HO3GHyDpO0InUUWzzOUrs52Mt6tEdkq7MuTokH0M8="
                },
                new Client
                {
                    Id = 2,
                    PublicKey = Guid.Parse("517dd6b8-8dc1-49e0-8e4d-2759379a8bc8"),
                    Name = "GK Client 2",
                    Slug = "gk-client-2",
                    Enabled = true,
                    PrimaryApiKey = "Q2xpZW50.Jo9C+6F3no9pwg8s1OWDkUGs+wHAVbsWkcRTS0s/SjU=",
                    SecondaryApiKey = "Q2xpZW50.FAe44G9HIrbDFCGa/ZF4xZE+m3Fne7cB7eNJuy2vcoc="
                },
                new Client
                {
                    Id = 3,
                    PublicKey = Guid.Parse("e0e4a4b2-c923-4cc5-9bcf-671221e56b3a"),
                    Name = "GK Client 3",
                    Slug = "gk-client-3",
                    Enabled = false,
                    PrimaryApiKey = "Q2xpZW50.Dpkt/TSLB+nlVyQwi/pSUhkwEglerntGUym6h+3DM/k=",
                    SecondaryApiKey = "Q2xpZW50.5/fRn0AL2RYPHcrT73HCSIuIYm2Iew5+1v9nvtXrtE4="
                }
            };
        }

        public IEnumerable<UserRefreshToken> GetFakeUserRefreshTokens()
        {
            return new List<UserRefreshToken>
            {
                new UserRefreshToken
                {
                    Id = 1,
                    PublicKey = Guid.Parse("8e483815-1f5a-4ae3-af15-b91cc9371878"),
                    Token = "VXNlcg==.dx9lkJq6WfEA+8oNoikmI4Mk3N/CeGY5hptngJSBmILXp8klx2A1vlzcWpieYa5xiUqimrTYAUrNTI0eQr84gQ==",
                    UserId = 1,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new UserRefreshToken
                {
                    Id = 2,
                    PublicKey = Guid.Parse("5a1f029f-d692-4148-b6a2-c6a072a71cdf"),
                    Token = "VXNlcg==.nBpRyGz2l+KtVOJO/Lr74MZs1IVh/Cl7hfE7+OO8V59IZlF+weU6BJiKz1L+sFLROZNyHi76ZPz7ZXOwYnsdXg==",
                    UserId = 1,
                    Created = DateTime.UtcNow.AddDays(-2),
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Revoked = DateTime.UtcNow.AddDays(-2)
                }
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
                    Token = "Q2xpZW50.VOJ6KaAss0+uQHD3e+OieAO2FrY2LsY/jzX6nAsSkdWqY7TGDI+b6lSsoNzvaMOwgu0TvXNPsQ7OlBBkuBYE0g==",
                    ClientId = 1,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new ClientRefreshToken
                {
                    Id = 2,
                    PublicKey = Guid.Parse("c8f80b28-3459-42b8-9c13-30e719a14df7"),
                    Token = "Q2xpZW50.9hMijUAPRBnEohxLg3z9VZRSefhuBfZs9NgkR3Bf9/r1WG1mupPNCJcdmgGWLBob7fRMCH4JFBJPiahYfQXYdA==",
                    ClientId = 2,
                    Created = DateTime.UtcNow.AddDays(-2),
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Revoked = DateTime.UtcNow.AddDays(-1)
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
                    Name = AiofClaims.Sub
                },
                new AiofClaim
                {
                    Id = 2,
                    Name = AiofClaims.Iss
                },new AiofClaim
                {
                    Id = 3,
                    Name = AiofClaims.PublicKey
                },
                new AiofClaim
                {
                    Id = 4,
                    Name = AiofClaims.GivenName
                },
                new AiofClaim
                {
                    Id = 5,
                    Name = AiofClaims.FamilyName
                },
                new AiofClaim
                {
                    Id = 6,
                    Name = AiofClaims.Name
                },
                new AiofClaim
                {
                    Id = 7,
                    Name = AiofClaims.Email
                },
                new AiofClaim
                {
                    Id = 8,
                    Name = AiofClaims.Slug
                },
            };
        }

        public IEnumerable<object[]> GetFakeUsersData(
            bool id = false,
            bool publicKey = false,
            bool password = false,
            bool firstName = false,
            bool lastName = false,
            bool email = false,
            bool username = false,
            bool apiKeys = false)
        {
            var fakeUsers = GetFakeUsers()
                .ToArray();

            var toReturn = new List<object[]>();

            if (firstName
                && lastName
                && email
                && username)
                foreach (var fakeUser in fakeUsers)
                {
                    toReturn.Add(new object[] 
                    { 
                        fakeUser.FirstName, 
                        fakeUser.LastName, 
                        fakeUser.Email, 
                        fakeUser.Username
                    });
                }
            else if (username 
                && password)
                return new List<object[]>
                {
                    new object[] { fakeUsers[0].Username, "pass1234" },
                    new object[] { fakeUsers[1].Username, "password123" }
                };
            else if (id)
                foreach (var fakeUserId in fakeUsers.Select(x => x.Id))
                {
                    toReturn.Add(new object[] 
                    { 
                        fakeUserId
                    });
                }
            else if (publicKey)
                foreach (var fakeUserPublicKey in fakeUsers.Select(x => x.PublicKey))
                {
                    toReturn.Add(new object[] 
                    { 
                        fakeUserPublicKey
                    });
                }
            else if (apiKeys)
                foreach (var fakeUser in fakeUsers
                    .Where(x => x.PrimaryApiKey != null && x.SecondaryApiKey != null))
                {
                    toReturn.Add(new object[] 
                    { 
                        fakeUser.PrimaryApiKey, fakeUser.SecondaryApiKey
                    });
                }

            return toReturn;
        }

        public IEnumerable<object[]> GetFakeClientsData(
            bool id = false,
            bool name = false,
            bool apiKey = false,
            bool enabled = true)
        {
            var fakeClients = GetFakeClients()
                .ToArray();

            var toReturn = new List<object[]>();

            if (id 
                && apiKey)
            {
                foreach (var fakeClient in fakeClients)
                    toReturn.Add(new object[]
                    {
                        fakeClient.Id,
                        fakeClient.PrimaryApiKey
                    });
            }
            else if (id
                && enabled)
            {
                foreach (var fakeClientId in fakeClients.Where(x => x.Enabled).Select(x => x.Id))
                    toReturn.Add(new object[]
                    {
                        fakeClientId
                    });
            }
            else if (id
                && !enabled)
            {
                foreach (var fakeClientId in fakeClients.Where(x => !x.Enabled).Select(x => x.Id))
                    toReturn.Add(new object[]
                    {
                        fakeClientId
                    });
            }
            else if (name)
            {
                foreach (var fakeClientName in fakeClients.Select(x => x.Name))
                    toReturn.Add(new object[]
                    {
                        fakeClientName
                    });
            }
            else if (apiKey)
            {
                foreach (var fakeClientPrimaryApiKey in fakeClients.Where(x => x.Enabled).Select(x => x.PrimaryApiKey))
                    toReturn.Add(new object[]
                    {
                        fakeClientPrimaryApiKey
                    });
            }
            
            return toReturn;
        }

        public IEnumerable<object[]> GetFakeUserRefreshTokensData(
            bool userId = false)
        {
            var refreshTokens = GetFakeUserRefreshTokens()
                .ToArray();

            var toReturn = new List<object[]>();

            if (userId)
            {
                foreach (var rtUserId in refreshTokens.Select(x => x.UserId).Distinct())
                    toReturn.Add(new object[]
                    {
                        rtUserId
                    });
            }

            return toReturn;
        }

        public IEnumerable<object[]> GetFakeClientRefreshTokensData(
            bool clientId = false,
            bool token = false,
            bool revoked = false)
        {
            var clientRefreshTokens = GetFakeClientRefreshTokens()
                .ToArray();

            var toReturn = new List<object[]>();

            if (clientId 
                && token
                && !revoked)
            {
                foreach (var clientRt in clientRefreshTokens.Where(x => x.Revoked == null))
                    toReturn.Add(new object[]
                    {
                        clientRt.ClientId,
                        clientRt.Token
                    });
            }
            else if (clientId 
                && token
                && revoked)
            {
                foreach (var clientRt in clientRefreshTokens.Where(x => x.Revoked != null))
                    toReturn.Add(new object[]
                    {
                        clientRt.ClientId,
                        clientRt.Token
                    });
            }
            else if (token
                && !revoked)
            {
                foreach (var clientRtToken in clientRefreshTokens.Where(x => x.Revoked == null).Select(x => x.Token))
                    toReturn.Add(new object[]
                    {
                        clientRtToken
                    });
            }
            else if (token
                && revoked)
            {
                foreach (var clientRtToken in clientRefreshTokens.Where(x => x.Revoked != null).Select(x => x.Token))
                    toReturn.Add(new object[]
                    {
                        clientRtToken
                    });
            }
            
            return toReturn;
        }

        public IEnumerable<object[]> GetFakeClaimsData()
        {
            var fakeClaims = GetFakeClaims()
                .ToArray();

            var toReturn = new List<object[]>();

            foreach (var fakeClaim in fakeClaims)
                toReturn.Add(new object[]
                    { 
                        fakeClaim.Id,
                        fakeClaim.PublicKey, 
                        fakeClaim.Name 
                    });
            
            return toReturn;
        }

        public string HashedPassword => "10000.JiFzc3Ijb5vBrCb8COiNzA==.BzdHomm3RMu0sMHaBfTpY0B2WtbjFqi9tN7T//N+khA="; //pass1234
        public string ExpiredJwtToken => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdWJsaWNfa2V5IjoiNTgxZjNjZTYtY2YyYS00MmE1LTgyOGYtMTU3YTJiZmFiNzYzIiwiZ2l2ZW5fbmFtZSI6Ikdlb3JnaSIsImZhbWlseV9uYW1lIjoiS2FtYWNoYXJvdiIsImVtYWlsIjoiZ2thbWFAdGVzdC5jb20iLCJlbnRpdHkiOiJVc2VyIiwibmJmIjoxNTk3MTc0NDQzLCJleHAiOjE1OTcxNzUzNDMsImlhdCI6MTU5NzE3NDQ0MywiaXNzIjoiYWlvZjphdXRoIiwiYXVkIjoiYWlvZjphdXRoOmF1ZGllbmNlIn0.d8-5b3ecDc4IBrFQfyr4sv1wqxh4MpOxsDExpXP8jG0";
    }
}