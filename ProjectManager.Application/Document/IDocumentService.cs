using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Application.Document;

public interface IDocumentService
{

    /// <summary>
    /// Uploading document to server
    /// </summary>
    Task UploadDocumentAsync(int projectId, UploadDocumentDto uploadDocumentDto);
    
    /// <summary>
    /// Downloading project from server
    /// </summary>
    /// <returns>FileName and FilePath in DTO</returns>
    Task<DownloadDocumentDto> DownloadDocumentAsync(int documentId);
}