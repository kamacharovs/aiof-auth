using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public class Role : IRole,
        IPublicKeyId
    {
        [Required]
        public int Id { get; set; }

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
    }
}