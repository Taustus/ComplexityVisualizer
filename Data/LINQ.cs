namespace Data
{
    public class LINQ<T> : BaseEntity where T : class
    {

        public static IEnumerable<T> Collection { get; protected set; }

        public override bool Any()
        {
            throw new NotImplementedException();
        }

        public override int Max()
        {
            throw new NotImplementedException();
        }

        public override int Min()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BaseEntity> Sort()
        {
            throw new NotImplementedException();
        }
    }
}