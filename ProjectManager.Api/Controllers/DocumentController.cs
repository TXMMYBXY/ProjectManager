using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Account.Auth;
using ProjectManager.Api.Features.Document.Requests;
using ProjectManager.Api.Features.Document.Responses;
using ProjectManager.Application.Document;
using ProjectManager.Application.Document.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/document")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IDocumentService _documentService;

    public DocumentController(IMapper mapper, IDocumentService documentService)
    {
        _mapper = mapper;
        _documentService = documentService;
    }

    [HttpGet("project-id/{projectId:int}")]
    public async Task<ActionResult<DocumentsResponse>> GetAllDocumentsByProjectId([FromRoute] int projectId)
    {
        var responseDto = await _documentService.GetAllDocumentsByProjectIdAsync(projectId);
        
        var response = _mapper.Map<DocumentsResponse>(responseDto);
        
        return Ok(response);
    }
    
    /// <summary>
    /// Endpoint for uploading document to server
    /// </summary>
    [HttpPost("{projectId:int}/upload")]
    [Authorize(Policy = Policy.DirectorAndManager)]
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

        var title = documentViewModel.FileName ?? string.Empty;
        var fileExt = Path.GetExtension(documentViewModel.FilePath ?? string.Empty);
        if (!string.IsNullOrEmpty(fileExt) && string.IsNullOrEmpty(Path.GetExtension(title)))
        {
            title = title + fileExt;
            documentViewModel.FileName = title;
        }

        var stream = new FileStream(documentViewModel.FilePath, FileMode.Open, FileAccess.Read);

        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(title, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(stream, contentType, documentViewModel.FileName);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policy.DirectorAndManager)]
    public async Task<ActionResult> DeleteDocument([FromRoute] int id)
    {
        await _documentService.DeleteDocumentByIdAsync(id);

        return NoContent();
    }
}