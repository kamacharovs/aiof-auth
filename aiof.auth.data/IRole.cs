using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IRole
    {
        [JsonIgnore]
        int Id { get; set; }

        [JsonIgnore]
        Guid PublicKey { get; set; }

        string Name { get; set; }
    }
}