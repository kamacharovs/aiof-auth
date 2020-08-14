using System;
using System.ComponentModel.DataAnnotations;

namespace aiof.auth.data
{
    public interface IEnable
    {
        [Required]
        bool Enabled { get; set; }
    }
}