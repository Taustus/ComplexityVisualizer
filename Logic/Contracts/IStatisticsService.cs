using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Contracts
{
    public interface IStatisticsService
    {
        float GetMedianByType(EnumerableType enumerableType, string method);
    }
}
