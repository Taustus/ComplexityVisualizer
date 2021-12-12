using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public abstract class RepositoryBase<C, T> : IRepositoryBase<T>, IDisposable
                          where T : class
                          where C : DbContext
    {
        public C RepositoryContext { get; protected set; }

        public RepositoryBase(C RepositoryContext) => this.RepositoryContext = RepositoryContext;

        public void AddRange(IEnumerable<T> entities)
        {
            RepositoryContext.Set<T>().AddRange(entities);
        }

        public void SaveChanges()
        {
            RepositoryContext?.SaveChanges();
        }

        public IEnumerable<T> GetAll() => RepositoryContext.Set<T>().AsNoTracking();

        public void Dispose()
        {
            RepositoryContext?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
