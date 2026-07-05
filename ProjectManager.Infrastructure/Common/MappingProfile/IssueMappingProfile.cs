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
            .ForAllMembers(opts=>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Entities.Models.Issue, IssueInfoDto>();
    }    
}