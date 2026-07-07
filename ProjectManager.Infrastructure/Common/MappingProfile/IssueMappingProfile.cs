using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Infrastructure.Common.MappingProfile;

public class IssueMappingProfile : Profile
{
    public IssueMappingProfile()
    {
        CreateMap<CreateIssueDto, Entities.Models.Issue>();
        
        CreateMap<UpdateIssueDto, Entities.Models.Issue>()
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status != null))
            .ForMember(dest => dest.Priority, opt => opt.Condition(src => src.Priority != null))
            .ForMember(dest => dest.AuthorId, opt => opt.Condition(src => src.AuthorId != null))
            .ForMember(dest => dest.ExecutorId, opt => opt.Condition(src => src.ExecutorId != null));

        CreateMap<Entities.Models.Issue, IssueInfoDto>();
    }    
}