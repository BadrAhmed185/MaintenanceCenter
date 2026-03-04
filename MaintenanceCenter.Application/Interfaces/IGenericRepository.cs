using System.Linq.Expressions;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();

        // Allows filtering (e.g., getting requests by status or by workshop)
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity); // Note: Our DbContext will intercept this and make it a soft-delete!
    }
}