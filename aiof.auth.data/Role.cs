using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class Role : IRole,
        IPublicKeyId
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
    }

    public static class Roles
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string Client = nameof(Client);
        public const string Basic = nameof(Basic);

        public const string AdminOrUser = Admin + "," + User;
        public const string AdminOrClient = Admin + "," + Client;
        public const string BasicOrUser = Basic + "," + User;
        public const string BasicOrClient = Basic + "," + Client;

        public static IEnumerable<string> All 
            => new string[]
            {
                Admin,
                User,
                Client,
                Basic
            };
    }
}