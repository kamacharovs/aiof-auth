using System;
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

        public const string AdminOrUser = nameof(Admin) + "," + nameof(User);
        public const string AdminOrClient = nameof(Admin) + "," + nameof(Client);
        public const string AdminOrUserOrClient = nameof(Admin) + "," + nameof(User) + "," + nameof(Client);
    }
}