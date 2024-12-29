using Microsoft.EntityFrameworkCore;

namespace aspnetapp;

public partial class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Account> Users { get; set; } = null!;
    public DbSet<GameData> GameData { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#if DEBUG
            var connstr = $"server=127.0.0.1;port=3306;user=root;password=123456;database=game";
#else
            var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME");
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            var addressParts = Environment.GetEnvironmentVariable("MYSQL_ADDRESS")?.Split(':');
            var host = addressParts?[0];
            var port = addressParts?[1];
            var connstr = $"server={host};port={port};user={username};password={password};database=game";
#endif
            optionsBuilder.UseMySql(connstr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.18-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .ToTable("account")
            .HasIndex(u => u.userId)
            .IsUnique();

        modelBuilder.Entity<GameData>()
            .ToTable("gamedata")
            .HasIndex(g => new { g.userId })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}