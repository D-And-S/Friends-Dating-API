using Friends_Date_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Friends_Date_API.Data
{
    // Represent the session with database
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        
        public DbSet<User> Users { get; set; }
    }
}
