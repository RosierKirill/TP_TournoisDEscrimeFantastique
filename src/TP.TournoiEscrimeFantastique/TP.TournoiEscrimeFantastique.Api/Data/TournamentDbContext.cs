using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data.Entities;

namespace TP.TournoiEscrimeFantastique.Api.Data;

public class TournamentDbContext(DbContextOptions<TournamentDbContext> options) : DbContext(options)
{
    public DbSet<PlayerEntity> Players { get; set; }
    public DbSet<MatchEntity> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntity>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.HasMany(p => p.Matches)
             .WithOne(m => m.Player)
             .HasForeignKey(m => m.PlayerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchEntity>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Outcome).IsRequired().HasMaxLength(10);
        });
    }
}
