using AutoMapper;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Infrastructure.Common.MappingProfile;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<UpdateProjectDto, Entities.Models.Project>()
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CreateProjectDto, Entities.Models.Project>();
    }
}