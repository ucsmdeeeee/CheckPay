using Microsoft.EntityFrameworkCore;
using WebApplication10.Models;

namespace WebApplication10.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Order_> Order_ { get; set; }
    }
}
