using Data.Contracts;
using Data.Repositories;

namespace Data
{
    public class StatisticsRepository : RepositoryBase<MySqlContext, StatisticsModel>, IStatisticsRepository
    {
        public StatisticsRepository(MySqlContext RepositoryContext) : base(RepositoryContext)
        {

        }
    }
}
