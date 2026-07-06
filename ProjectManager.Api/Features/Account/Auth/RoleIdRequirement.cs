using Microsoft.AspNetCore.Authorization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Account.Auth;

public class RoleIdRequirement : IAuthorizationRequirement
{
    public UserRole[] AllowedRoles { get; }

    public RoleIdRequirement(params UserRole[] roles)
    {
        AllowedRoles = roles ?? Array.Empty<UserRole>();
    }
}
