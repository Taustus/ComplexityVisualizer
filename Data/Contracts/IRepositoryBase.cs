namespace Data
{
    public interface IRepositoryBase<T>
    {
        void AddRange(IEnumerable<T> entities);

        IEnumerable<T> GetAll();
    }
}
