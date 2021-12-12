using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("StatisticsModel")]
    public class StatisticsModel
    {
        [Key]
        public long Id { get; set; }

        public long Ticks { get; protected set; }

        [StringLength(3)]
        public string Method { get; protected set; }

        [StringLength(11)]
        public string EnumerableType { get; protected set; }

        public StatisticsModel(long Ticks, string Method, string EnumerableType)
        {
            this.Ticks = Ticks;
            this.Method = Method;
            this.EnumerableType = EnumerableType;
        }
    }
}
