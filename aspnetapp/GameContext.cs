using Microsoft.EntityFrameworkCore;

namespace aspnetapp;

public partial class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<GameData> GameData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasIndex(u => u.OpenId)
            .IsUnique();

        modelBuilder.Entity<GameData>()
            .ToTable("GameData")
            .HasIndex(g => new { g.UserId, g.Key })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
