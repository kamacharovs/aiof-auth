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
        [Required]
        public int Id { get; set; }

        [JsonIgnore]
        [Required]
        public Guid PublicKey { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public static class Roles
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string Client = nameof(Client);
        public const string Basic = nameof(Basic);


        public static string AdminOrUser = Combine(Admin, User);
        public static string AdminOrClient = Combine(Admin, Client);
        public static string BasicOrUser = Combine(Basic, User);
        public static string BasicOrClient = Combine(Basic, Client);
        public static string Any = Combine(All.ToArray());
        public static string Combine(params string[] roles) => string.Join(",", roles);

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