using Data.Enums;

namespace Logic.Contracts
{
    public interface IStatisticsService
    {
        void RunTasks();

        float GetMedianByType(EnumerableType enumerableType, string method);
    }
}
