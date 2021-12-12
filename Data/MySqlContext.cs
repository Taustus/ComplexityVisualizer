using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MySqlContext : DbContext
    {
        internal virtual DbSet<StatisticsModel>? StatisticsModel { get; private set; }

        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options) { }
    }
}
