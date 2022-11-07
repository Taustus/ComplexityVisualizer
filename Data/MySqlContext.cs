using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MySqlContext : DbContext
    {
        public virtual DbSet<StatisticsModel>? StatisticsModels { get; set; }

        public MySqlContext()
        {

        }

        public MySqlContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;userid=root;password=1123581321;database=Statistics;",
                                        MySqlServerVersion.LatestSupportedServerVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatisticsModel>()
                        .HasKey(e => e.Id);
        }
    }
}
