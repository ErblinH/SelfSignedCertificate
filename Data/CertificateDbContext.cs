using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class CertificateDbContext : DbContext
{
    public CertificateDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Certificate> Certificates { get; set; }
}