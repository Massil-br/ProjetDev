using Microsoft.EntityFrameworkCore;

namespace src.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
    }
}