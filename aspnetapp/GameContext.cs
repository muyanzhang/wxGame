using Microsoft.EntityFrameworkCore;

namespace aspnetapp;

public partial class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<GameData> GameData { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME");
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            var addressParts = Environment.GetEnvironmentVariable("MYSQL_ADDRESS")?.Split(':');
            var host = addressParts?[0];
            var port = addressParts?[1];
            var connstr = $"server={host};port={port};user={username};password={password};database=game";
            optionsBuilder.UseMySql(connstr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.18-mysql"));
        }
    }

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
