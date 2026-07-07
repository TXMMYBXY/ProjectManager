using AutoMapper;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Document;
using ProjectManager.Application.Document.Dto;
using ProjectManager.Application.Project;

namespace ProjectManager.Infrastructure.Document;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        IFileStorageService fileStorageService)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _fileStorageService = fileStorageService;
    }
    
    public async Task UploadDocumentAsync(int projectId, UploadDocumentDto uploadDocumentDto)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        
        ConflictException.ThrowIf(project is null, "Project is not exists");
        ConflictException.ThrowIf(uploadDocumentDto is null, "File is not exists");

        if (uploadDocumentDto.FileLength == 0)
        {
            throw new ArgumentException("File is empty");
        }
        
        var uniqueFileName = $"{Guid.NewGuid()}_{uploadDocumentDto.FileName}";

        var projectFolder = $"{project.Id}_{_ClearName(project.Title)}";

        var filePath = await _fileStorageService.SaveFileAsync(
            uploadDocumentDto.FileStream,
            uniqueFileName,
            projectFolder);

        var document = new Entities.Models.Document
        {
            Title = uploadDocumentDto.FileName,
            FilePath = filePath,
            ProjectId = projectId
        };

        await _documentRepository.CreateAsync(document);
        await _documentRepository.SaveChangesAsync();
    }

    public async Task<DownloadDocumentDto> DownloadDocumentAsync(int documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);

        NotFoundException.ThrowIfNull(document, "Document not found");

        return new DownloadDocumentDto
        {
            FilePath = document.FilePath,
            FileName = document.Title
        };
    }

    private string _ClearName(string input)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            input = input.Replace(c, '_');

        return input.Replace(" ", "_");
    }
}