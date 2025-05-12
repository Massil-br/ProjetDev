

namespace src.Models
{
    public class User
    {
        public int Id{get; set;}
        public required string UserName {get;set;}
        public required string  Email {get;set;} // unique 
        public  required string PasswordHash {get;set;}


    }



}