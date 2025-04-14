using System;
using System.Text.RegularExpressions;

namespace Shared
{
    public class User
    {
        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

     
        public User(int ID, string Username, string Email, string Password)
        {
            this.ID = ID;
            this.Username = Username;
            this.Email = Email;
            this.Password = BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 12);
        }

       
    }
}
