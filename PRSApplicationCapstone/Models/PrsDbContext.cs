using Microsoft.EntityFrameworkCore;

namespace PRSApplicationCapstone.Models
{
    public class PrsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestLine> RequestLines { get; set; } 
        public PrsDbContext(DbContextOptions<PrsDbContext> options) : base(options) { }
    }
}
