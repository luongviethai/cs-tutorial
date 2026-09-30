using Microsoft.EntityFrameworkCore;
using CS_Tutorial.Models;

namespace CS_Tutorial.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Todo> Todos { get; set; }
}