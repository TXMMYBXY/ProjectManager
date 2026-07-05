namespace ProjectManager.Application.Account;

public interface IJwtService
{
    string GenerateToken(Entities.Models.Employee employee);
}