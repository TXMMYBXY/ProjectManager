using AutoMapper;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Infrastructure.Common.MappingProfile;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<CreateEmployeeDto, Entities.Models.Employee>();
        
        CreateMap<UpdateEmployeeDto, Entities.Models.Employee>()
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Entities.Models.Employee, EmployeeInfoDto>();

        CreateMap<Entities.Models.Employee, EmployeeItemDto>();
    }
}