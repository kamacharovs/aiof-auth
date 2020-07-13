using System;
using System.Text.Json.Serialization;

namespace aiof.auth.data
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PrimaryApiKey { get; set; }
        public string SecondaryApiKey { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
