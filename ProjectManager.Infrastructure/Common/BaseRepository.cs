using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Common;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public BaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>()
            .Where(e => EF.Property<int>(e, "Id") == id)
            .SingleOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
    }

    public async Task<int> DeleteByIdAsync(int entityId)
    {
        return await _dbContext.Set<T>()
            .Where(e => EF.Property<int>(e, "Id") == entityId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids)
    {
        return await _dbContext.Set<T>()
            .Where(e => ids.Contains(EF.Property<int>(e,"Id")))
            .ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Set<T>().CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}