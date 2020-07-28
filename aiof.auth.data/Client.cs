using System;

namespace aiof.auth.data
{
    public class Client : IClient,
        IPublicKeyId, IApiKey
    {
        public int Id { get; set; }
        public Guid PublicKey { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Enabled { get; set; } = true;
        public string PrimaryApiKey { get; set; }
        public string SecondaryApiKey { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public class ClientDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Enabled { get; set; } = true;
    }
}