using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public interface IUser
    {
        [JsonIgnore] int Id { get; set; }
        [JsonIgnore] Guid PublicKey { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        DateTime Created { get; set; }
    }
}