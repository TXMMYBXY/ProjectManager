using AutoMapper;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Common.MappingProfile;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<CreateProjectDto, Entities.Models.Project>();
        
        CreateMap<UpdateProjectDto, Entities.Models.Project>()
            .ForMember(dest => dest.FinishDate, opt => opt.Ignore())
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Entities.Models.Project, ProjectInfoDto>();
    }
}