using ProjectManager.Application.Document;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Document;

public class DocumentRepository : BaseRepository<Entities.Models.Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}