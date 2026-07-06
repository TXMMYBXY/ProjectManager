using AutoMapper;
using ProjectManager.Api.Features.Account.Requests;
using ProjectManager.Api.Features.Account.Responses;
using ProjectManager.Application.Account.Dto;

namespace ProjectManager.Api.Features.Account;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<RegisterRequest, RegisterDto>();

        CreateMap<RegisterResultDto, RegisterResponse>();
        
        CreateMap<LoginRequest, LoginDto>();
        
        CreateMap<LoginResultDto, LoginResponse>();
    }
}