using ProjectManager.Application.Common;
using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Application.Document;

public interface IDocumentRepository : IBaseRepository<Entities.Models.Document>
{
    Task<IReadOnlyList<DocumentItemDto>> GetAllDocumentsByProjectIdAsync(int projectId);
}