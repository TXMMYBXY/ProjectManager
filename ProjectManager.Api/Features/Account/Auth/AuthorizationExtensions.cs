using Microsoft.AspNetCore.Authorization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Account.Auth;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.DirectorOnly, policy => policy.Requirements.Add(
                new RoleIdRequirement(UserRole.Director)));
            
            options.AddPolicy(Policy.DirectorAndManager, policy => policy.Requirements.Add(
                new RoleIdRequirement(UserRole.Director, UserRole.Manager)));

            options.AddPolicy(Policy.All, policy => policy.Requirements.Add(
                new RoleIdRequirement(UserRole.Director, UserRole.Manager, UserRole.Employee)));
        });
        
        services.AddSingleton<IAuthorizationHandler, RoleHandler>();
        
        return services;
    }
}