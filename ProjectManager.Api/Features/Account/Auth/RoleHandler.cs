using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Account.Auth;

public class RoleHandler : AuthorizationHandler<RoleIdRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, RoleIdRequirement requirement)
    {
        var roleClaims = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        foreach (var value in roleClaims)
        {
            if (Enum.TryParse<UserRole>(value, ignoreCase: true, out var parsedRole))
            {
                if (requirement.AllowedRoles.Contains(parsedRole))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            else if (int.TryParse(value, out var numeric))
            {
                if (Enum.IsDefined(typeof(UserRole), numeric))
                {
                    var enumFromNumber = (UserRole)numeric;
                    if (requirement.AllowedRoles.Contains(enumFromNumber))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }
        }

        var roleIdClaim = context.User.FindFirst("roleId")?.Value;
        
        if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var roleIdNum))
        {
            if (Enum.IsDefined(typeof(UserRole), roleIdNum))
            {
                var enumRole = (UserRole)roleIdNum;
                if (requirement.AllowedRoles.Contains(enumRole))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }
}