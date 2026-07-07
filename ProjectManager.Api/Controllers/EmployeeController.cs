using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Features.Account.Auth;
using ProjectManager.Api.Features.Employee.Requests;
using ProjectManager.Api.Features.Employee.Responses;
using ProjectManager.Api.Features.Project.Requests;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/employee")]
[Authorize(Policy = Policy.DirectorAndManager)]
public class EmployeeController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IMapper mapper, IEmployeeService employeeService)
    {
        _mapper = mapper;
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedEmployeeResponse>> GetAllEmployees([FromQuery] GetEmployeesRequest request)
    {
        var filter = _mapper.Map<EmployeeFilter>(request);
        
        var responseDto = await _employeeService.GetAllEmployeesAsync(filter);
        
        var response = _mapper.Map<PagedEmployeeResponse>(responseDto);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeInfoResponse>> GetEmployeeById([FromRoute] int id)
    {
        var responseDto = await _employeeService.GetEmployeeByIdAsync(id);
        
        var response = _mapper.Map<EmployeeInfoResponse>(responseDto);

        return Ok(response);
    }

    [HttpGet("project-managers")]
    public async Task<ActionResult<ProjectManagerResponse>> GetProjectManagersAndDirectors()
    {
        var responseDto = await _employeeService.GetProjectManagersAsync();

        var response = _mapper.Map<ProjectManagerResponse>(responseDto);

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = Policy.DirectorOnly)]
    public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var employeeDto = _mapper.Map<CreateEmployeeDto>(request);

        await _employeeService.CreateEmployeeAsync(employeeDto);

        return Created();
    }

    [HttpPost("insert/{employeeId:int}/{projectId}")]
    [Authorize(Policy = Policy.DirectorAndManager)]
    public async Task<ActionResult<EmployeeInfoResponse>> AddProjectToEmployee([FromRoute] int projectId, 
        [FromRoute] int employeeId)
    {
        var responseDto = await _employeeService.AddProjectToEmployeeAsync(projectId, employeeId);

        var response = _mapper.Map<EmployeeInfoResponse>(responseDto);
        
        return Ok(response);
    }

    [HttpPost("bulk-insert/{employeeId:int}")]
    [Authorize(Policy = Policy.DirectorAndManager)]
    public async Task<ActionResult<int>> BulkInsertProjectsToEmployee([FromBody] BulkInsertRequest request,
        [FromRoute] int employeeId)
    {
        var response = await _employeeService.BulkInsertProjectsToEmployeeAsync(request.Ids, employeeId);

        return Ok(response);
    }

    [HttpPost("delete/{employeeId:int}/{projectId}")]
    [Authorize(Policy = Policy.DirectorAndManager)]
    public async Task<ActionResult<bool>> DeleteProjectFromEmployee([FromRoute] int projectId, [FromRoute] int employeeId)
    {
        var response = await _employeeService.DeleteProjectFromEmployeeAsync(projectId, employeeId);

        return Ok(response);
    }

    [HttpPost("bulk-delete/{employeeId:int}")]
    [Authorize(Policy = Policy.DirectorAndManager)]
    public async Task<ActionResult<int>> BulkDeleteProjectsFromEmployee([FromBody] BulkDeleteRequest request,
        [FromRoute] int employeeId)
    {
        var response = await _employeeService.BulkDeleteProjectsFromEmployeeAsync(request.Ids, employeeId);

        return Ok(response);
    }

    [HttpPost("bulk-delete")]
    [Authorize(Policy = Policy.DirectorOnly)]
    public async Task<ActionResult<int>> BulkDeleteEmployees([FromBody] BulkDeleteRequest request)
    {
        var response = await _employeeService.BulkDeleteEmployeesAsync(request.Ids);
        
        return Ok(response);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = Policy.DirectorOnly)]
    public async Task<ActionResult<UpdateEmployeeResponse>> UpdateEmployee([FromRoute] int id,
        [FromBody] UpdateEmployeeRequest request)
    {
        var requestDto = _mapper.Map<UpdateEmployeeDto>(request);
        
        var responseDto = await _employeeService.UpdateEmployeeAsync(id, requestDto);
        
        var response = _mapper.Map<UpdateEmployeeResponse>(responseDto);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policy.DirectorOnly)]
    public async Task<ActionResult> DeleteEmployeeById([FromRoute] int id)
    {
        await _employeeService.DeleteEmployeeByIdAsync(id);
        
        return NoContent();
    }
}