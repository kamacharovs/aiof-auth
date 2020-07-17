using System;

namespace aiof.auth.data
{
    public interface IClient
    {
        int Id { get; set; }
        Guid PublicKey { get; set; }
        string Name { get; set; }
        string Slug { get; set; }
        bool Enabled { get; set; }
        string PrimaryApiKey { get; set; }
        string SecondaryApiKey { get; set; }
        DateTime Created { get; set; }
    }
}