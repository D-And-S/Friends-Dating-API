using Friends_Date_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Friends_Date_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        

        public DbSet<AppUser> Users { get; set; }
    }
}
