namespace ProjectManager.Application.Common;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task CreateAsync(T entity);
    Task<int> DeleteAsync(int entityId);
    Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids);
    Task<int> CountAsync();
    Task SaveChangesAsync();
}