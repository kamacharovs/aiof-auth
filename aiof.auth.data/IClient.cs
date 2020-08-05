using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public interface IClient
    {
        [JsonIgnore] int Id { get; set; }
        [JsonIgnore] Guid PublicKey { get; set; }
        string Name { get; set; }
        string Slug { get; set; }
        bool Enabled { get; set; }
        string PrimaryApiKey { get; set; }
        string SecondaryApiKey { get; set; }
        DateTime Created { get; set; }
    }
}