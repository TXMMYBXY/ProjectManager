using AutoMapper;
using ProjectManager.Api.Features.Employee.Requests;
using ProjectManager.Api.Features.Employee.Responses;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Api.Features.Employee;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<GetEmployeesRequest, EmployeeFilter>();
        
        CreateMap<PagedEmployeeDto, PagedEmployeeResponse>();

        CreateMap<EmployeeInfoDto, EmployeeInfoResponse>();
        
        CreateMap<CreateEmployeeRequest, CreateEmployeeDto>();
        
        CreateMap<UpdateEmployeeRequest, UpdateEmployeeDto>();
        
        CreateMap<EmployeeInfoDto, UpdateEmployeeResponse>();
    }
}