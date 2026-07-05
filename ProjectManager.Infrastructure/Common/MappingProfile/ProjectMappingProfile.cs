using AutoMapper;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Infrastructure.Common.MappingProfile;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<CreateProjectDto, Entities.Models.Project>();
        
        CreateMap<UpdateProjectDto, Entities.Models.Project>()
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Entities.Models.Project, ProjectInfoDto>();
    }
}