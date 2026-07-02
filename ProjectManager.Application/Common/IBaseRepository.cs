namespace ProjectManager.Application.Common;

public interface IBaseRepository<T> where T : class
{
    Task CreateAsync(T entity);

    Task<int> DeleteAsync(int entityId);

    Task<int> CountAsync();

    Task SaveChangesAsync();
}