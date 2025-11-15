using DailyStandup.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyStandup.Infrastructure.Persistence
{
    public class DailyStandupDbContext : DbContext
    {
        public DailyStandupDbContext(DbContextOptions<DailyStandupDbContext> options) : base(options) { }
        public DbSet<LogEntry> LogEntries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Message).IsRequired().HasMaxLength(4000);
                b.Property(x => x.Level).HasMaxLength(50);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
