using ProjectManager.Application.Account.Dto;

namespace ProjectManager.Application.Account;

public interface IAccountService
{
    Task<RegisterResultDto> RegisterAsync(RegisterDto dto);
    Task<LoginResultDto> LoginAsync(LoginDto dto);
    Task LogoutAsync();
}