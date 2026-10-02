using Microsoft.EntityFrameworkCore;
using CS_Tutorial.Models;

namespace CS_Tutorial.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Email).HasMaxLength(256);
            e.HasIndex(u => u.Email).IsUnique();

            e.Property(u => u.Role).HasMaxLength(20);
        });

        base.OnModelCreating(modelBuilder);
    }
}