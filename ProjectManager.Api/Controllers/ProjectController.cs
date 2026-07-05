using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Project.Requests;
using ProjectManager.Api.Features.Project.Responses;
using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/project")]
public class ProjectController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProjectService _projectService;

    public ProjectController(IMapper mapper, IProjectService projectService)
    {
        _mapper = mapper;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedProjectResponse>> GetAllProjects([FromQuery] GetProjectsRequest request)
    {
        var filter = _mapper.Map<ProjectFilter>(request);
        
        var responseDto = await _projectService.GetAllProjectsAsync(filter);
        
        var response = _mapper.Map<PagedProjectResponse>(responseDto);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectInfoResponse>> GetProjectById([FromRoute] int id)
    {
        var responseDto = await _projectService.GetProjectByIdAsync(id);
        
        var response = _mapper.Map<ProjectInfoResponse>(responseDto);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var requestDto = _mapper.Map<CreateProjectDto>(request);
        
        await _projectService.CreateProjectAsync(requestDto);

        return Created();
    }

    [HttpPost("bulk-delete")]
    public async Task<ActionResult<int>> BulkDeleteProjects([FromBody] BulkDeleteRequest request)
    {
        var response = await _projectService.BulkDeleteProjectsAsync(request.Ids);

        return Ok(response);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<UpdateProjectResponse>> UpdateProjectResponse([FromRoute] int id, [FromBody] UpdateProjectRequest request)
    {
        var requestDto = _mapper.Map<UpdateProjectDto>(request);
        
        var responseDto = await _projectService.UpdateProjectAsync(id, requestDto);

        var response = _mapper.Map<UpdateProjectResponse>(responseDto);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProjectById([FromRoute] int id)
    {
        await _projectService.DeleteProjectByIdAsync(id);
        
        return NoContent();
    }
}