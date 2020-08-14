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
                },
                new Client
                {
                    Id = 3,
                    PublicKey = Guid.Parse("e0e4a4b2-c923-4cc5-9bcf-671221e56b3a"),
                    Name = "GK Client 3",
                    Slug = "gk-client-3",
                    Enabled = false,
                    PrimaryApiKey = "gk-client-3-p-key",
                    SecondaryApiKey = "gk-client-3-s-key"
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
                    Token = "refresh-token-1",
                    ClientId = 1,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(1)
                },
                new ClientRefreshToken
                {
                    Id = 2,
                    PublicKey = Guid.Parse("c8f80b28-3459-42b8-9c13-30e719a14df7"),
                    Token = "refresh-token-2",
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
            bool username = false)
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