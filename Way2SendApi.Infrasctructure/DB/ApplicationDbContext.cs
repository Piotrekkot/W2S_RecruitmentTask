using Microsoft.EntityFrameworkCore;
using Way2SendApi.Infrastructure.Models;

namespace Way2SendApi.Infrastructure.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
    }
}
