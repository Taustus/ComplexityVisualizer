namespace Data
{
    public interface IRepositoryBase<T>
    {
        void AddRange(IEnumerable<T> entities);

        void SaveChanges();

        IEnumerable<T> GetAll();
    }
}
