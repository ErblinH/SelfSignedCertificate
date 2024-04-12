using Microsoft.EntityFrameworkCore;

namespace Data;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Test> Tests { get; set; }
}