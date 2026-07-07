using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Document;
using ProjectManager.Application.Document.Dto;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Document;

public class DocumentRepository : BaseRepository<Entities.Models.Document>, IDocumentRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DocumentItemDto>> GetAllDocumentsByProjectIdAsync(int projectId)
    {
        return await _dbContext.Documents
            .AsNoTracking()
            .Where(d => d.ProjectId == projectId)
            .Select(d => new DocumentItemDto
            {
                Id = d.Id,
                Title = d.Title
            })
            .ToListAsync();
    }
}