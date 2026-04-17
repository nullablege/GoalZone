using Microsoft.EntityFrameworkCore;
using GoalZone.API.Entities;

namespace GoalZone.API.Contexts
{
    public class AppDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;initial catalog=GoalZoneSportmonks;integrated security=true;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Team> Team { get; set; }
        public DbSet<Season> Season { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<MatchEvent> MatchEvent { get; set; }
        public DbSet<MatchStatistic> MatchStatistic { get; set; }
        public DbSet<MatchSubstitution> MatchSubstitution { get; set; }
        public DbSet<MatchInfo> MatchInfo { get; set; }

        //
        public DbSet<StandingCorrection> StandingCorrections { get; set; }

    }
}
