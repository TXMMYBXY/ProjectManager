using AutoMapper;
using ProjectManager.Api.Features.Issue.Requests;
using ProjectManager.Api.Features.Issue.Responses;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Api.Features.Issue;

public class IssueMappingProfile : Profile
{
    public IssueMappingProfile()
    {
        CreateMap<GetIssuesRequest, IssueFilter>();

        CreateMap<PagedIssueDto, PagedIssueResponse>();

        CreateMap<IssueInfoDto, IssueInfoResponse>();
        
        CreateMap<CreateIssueRequest, CreateIssueDto>();
        
        CreateMap<UpdateIssueRequest, UpdateIssueDto>();
        
        CreateMap<UpdateIssueDto, UpdateIssueResponse>();
    }
}