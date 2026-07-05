using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Issue.Requests;
using ProjectManager.Api.Features.Issue.Responses;
using ProjectManager.Api.Features.Project.Requests;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/issue")]
[Authorize]
public class IssueController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IIssueService _issueService;

    public IssueController(IMapper mapper, IIssueService issueService)
    {
        _mapper = mapper;
        _issueService = issueService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedIssueResponse>> GetAllIssues([FromQuery] GetIssuesRequest request)
    {
        var filter = _mapper.Map<IssueFilter>(request);
        
        var responseDto = await _issueService.GetAllIssuesAsync(filter);

        var response = _mapper.Map<PagedIssueResponse>(responseDto);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IssueInfoResponse>> GetIssueById([FromRoute] int id)
    {
        var responseDto = await _issueService.GetIssueByIdAsync(id);
        
        var response = _mapper.Map<IssueInfoResponse>(responseDto);

        return Ok(response);
    }
    
    [HttpPost]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> CreateIssue([FromBody] CreateIssueRequest request)
    {
        var requestDto = _mapper.Map<CreateIssueDto>(request);
        
        requestDto.AuthorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        await _issueService.CreateIssueAsync(requestDto);

        return Created();
    }

    [HttpPost("bulk-delete")]
    public async Task<ActionResult<int>> BulkDeleteIssues([FromBody] BulkDeleteRequest request)
    {
        var response = await _issueService.BulkDeleteIssuesAsync(request.Ids);

        return Ok(response);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<UpdateIssueResponse>> UpdateResponse([FromRoute] int id,
        [FromBody] UpdateIssueRequest request)
    {
        var requestDto = _mapper.Map<UpdateIssueDto>(request);
        
        var responseDto = await _issueService.UpdateIssueAsync(id, requestDto);

        var response = _mapper.Map<UpdateIssueResponse>(responseDto);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteIssueById([FromRoute] int id)
    {
        await _issueService.DeleteIssueByIdAsync(id);
        
        return NoContent();
    }
}