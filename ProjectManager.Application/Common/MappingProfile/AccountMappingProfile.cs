using AutoMapper;
using ProjectManager.Application.Account.Dto;

namespace ProjectManager.Application.Common.MappingProfile;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<RegisterDto, Entities.Models.Employee>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}