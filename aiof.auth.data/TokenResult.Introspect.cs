using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class IntrospectTokenResult
{
    [Required]
    public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

    [Required]
    public string Status { get; set; }
}