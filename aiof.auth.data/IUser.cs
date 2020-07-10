using System;

namespace aiof.auth.data
{
    public interface IUser
    {
        int Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Username { get; set; }
        string ApiKey { get; set; }
        string Password { get; set; }
    }
}