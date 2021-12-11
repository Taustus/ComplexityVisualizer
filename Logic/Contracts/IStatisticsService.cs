using Data.Enums;

namespace Logic.Contracts
{
    public interface IStatisticsService
    {
        float GetMedianByType(EnumerableType enumerableType, string method);
    }
}
