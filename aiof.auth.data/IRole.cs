using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IRole
    {
        [Required]
        int Id { get; set; }

        [Required]
        Guid PublicKey { get; set; }

        [Required]
        [MaxLength(50)]
        string Name { get; set; }
    }
}