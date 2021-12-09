using Data.Enums;

namespace Logic
{
    

    public class CollectionWorker<T>
    {
        public IEnumerable<T> Collection { get; protected set; }

        public EnumerableType EnumerableType { get; protected set; }

        public Random Randomizer { get; protected set; }

        public CollectionWorker(EnumerableType EnumerableType, CollectionSize CollectionSize)
        {
            Randomizer = new Random();
            this.EnumerableType = EnumerableType;

            Collection = GenerateCollection(CollectionSize);
        }

        public T Max()
        {
            var value = Collection.Max();

            if(value is null)
            {
                throw new ArgumentException("Problem finding max value!", Collection.GetType().ToString());
            }
            else
            {
                return value;
            }
        }

        public T Min()
        {
            var value = Collection.Min();

            if (value is null)
            {
                throw new ArgumentException("Problem finding min value!", Collection.GetType().ToString());
            }
            else
            {
                return value;
            }
        }

        public bool Any() => Collection.Any();

        private IEnumerable<T> GenerateCollection(CollectionSize collectionSize)
        {
            var size = (int)collectionSize;
            IEnumerable<T> collection = EnumerableType switch
            {
                EnumerableType.List => new List<T>(),
                EnumerableType.IEnumerable => Enumerable.Empty<T>(),
                EnumerableType.Array => Enumerable.Empty<T>().ToArray(),
                EnumerableType.HashSet => new HashSet<T>(),
                _ => Enumerable.Empty<T>(),
            };

            for (int i = 0; i < size; i++)
            {
                var value = GenerateValue();

                collection = collection.Append(value);
            }

            return collection;
        }

        private T GenerateValue()
        {
            dynamic? value = null;
            dynamic randomNumber;

            if (typeof(T) == typeof(string))
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                randomNumber = Randomizer.Next(0, chars.Length);
                value = chars[(int)randomNumber].ToString();
            }
            else if (typeof(T) == typeof(char))
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                randomNumber = Randomizer.Next(0, chars.Length);
                value = chars[(int)randomNumber];
            }
            else if (typeof(T) == typeof(int))
            {
                randomNumber = Randomizer.Next(int.MinValue, int.MaxValue);
                value = randomNumber;
            }
            else if (typeof(T) == typeof(long))
            {
                randomNumber = Randomizer.NextInt64();
                value = randomNumber;
            }
            else if (typeof(T) == typeof(bool))
            {
                randomNumber = Randomizer.Next(0, 2);
                value = randomNumber == 1;
            }

            if(value is null)
            {
                throw new ArgumentException("This type is not supported!", typeof(T).Name);
            }
            else
            {
                return value;
            }
        }
    }
}