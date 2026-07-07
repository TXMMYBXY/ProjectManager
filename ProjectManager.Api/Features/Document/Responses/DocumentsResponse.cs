using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Api.Features.Document.Responses;

public class DocumentsResponse
{
    public IReadOnlyList<DocumentItemDto> Documents { get; set; }
}