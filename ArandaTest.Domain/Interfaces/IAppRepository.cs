namespace ArandaTest.Domain.Interfaces
{
    public interface IAppRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, bool includeNavigations = false);
        Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? sort = null,
            int page = 1,
            int pageSize = 10, bool includeNavigations = false);
        Task<int> GetCountAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
