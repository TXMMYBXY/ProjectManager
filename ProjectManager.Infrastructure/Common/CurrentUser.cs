using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectManager.Application.Common;

namespace ProjectManager.Infrastructure.Common;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException();

    public int Id
    {
        get
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(val) || !int.TryParse(val, out var id))
                throw new UnauthorizedAccessException();

            return id;
        }
    }

    public bool IsInRole(string role) => User.IsInRole(role);

}