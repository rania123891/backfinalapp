using Microsoft.EntityFrameworkCore;
using MessageService.Domain.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MessageService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Message>().HasKey(m => m.Id);
    }
}
