using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enums
{
    public enum CollectionSize
    {
        Tiny = 1000,
        Small = 10000,
        Medium = 100000,
        Large = 1000000,
        Huge = 10000000
    }

    public enum EnumerableType
    {
        List,
        IEnumerable,
        Array,
        HashSet,
    }

    public enum ProcessTime
    {
        Tiny = 1000,
        Short = 3600000,
        Long = 36000000,
        Infinite = int.MaxValue,
    }
}
