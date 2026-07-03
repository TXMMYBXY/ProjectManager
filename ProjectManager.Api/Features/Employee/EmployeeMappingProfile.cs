using AutoMapper;
using ProjectManager.Api.Features.Employee.Requests;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Api.Features.Employee;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<CreateEmployeeRequest, CreateEmployeeDto>();
    }
}