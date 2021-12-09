using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class BaseEntity
    {
        public abstract int Max();

        public abstract int Min();

        public abstract bool Any();

        public abstract IEnumerable<BaseEntity> Sort();
    }
}
