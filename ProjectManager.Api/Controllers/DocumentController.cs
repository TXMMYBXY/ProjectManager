using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Document;
using ProjectManager.Application.Document;
using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/document")]
public class DocumentController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IDocumentService _documentService;

    public DocumentController(IMapper mapper, IDocumentService documentService)
    {
        _mapper = mapper;
        _documentService = documentService;
    }
    
    /// <summary>
    /// Endpoint for uploading document to server
    /// </summary>
    [HttpPost("{projectId:int}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> AddDocumentToProject([FromRoute] int projectId, 
        [FromForm]UploadDocumentRequest uploadDocumentViewModel)
    {
        var uploadDocumentDto = _mapper.Map<UploadDocumentDto>(uploadDocumentViewModel);
        
        uploadDocumentDto.FileStream = uploadDocumentViewModel.File.OpenReadStream();
        
        await _documentService.UploadDocumentAsync(projectId, uploadDocumentDto);

        return Ok();
    }
    
    /// <summary>
    /// Endpoint for downloading document from server
    /// </summary>
    [HttpGet("{documentId:int}/download")]
    public async Task<IActionResult> GetDocument([FromRoute] int documentId)
    {
        var documentDto = await _documentService.DownloadDocumentAsync(documentId);
        
        var documentViewModel = _mapper.Map<DownloadDocumentResponse>(documentDto);
        
        var stream = new FileStream(documentViewModel.FilePath, FileMode.Open, FileAccess.Read);

        return File(stream, "application/octet-stream", documentViewModel.FileName);
    }
}