using AutoMapper;
using ProjectManager.Api.Features.Project.Requests;
using ProjectManager.Api.Features.Project.Responses;
using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Api.Features.Project;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<GetProjectsRequest, ProjectFilter>();

        CreateMap<PagedProjectDto, PagedProjectResponse>();
        
        CreateMap<CreateProjectRequest, CreateProjectDto>();
        
        CreateMap<UpdateProjectRequest, UpdateProjectDto>()
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ProjectInfoDto, UpdateProjectResponse>();
        
        CreateMap<ProjectInfoDto, ProjectInfoResponse>();
    }
}