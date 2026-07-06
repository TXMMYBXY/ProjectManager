using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.Application.Account;
using ProjectManager.Infrastructure.Configuration;

namespace ProjectManager.Infrastructure.Account;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    public string GenerateToken(Entities.Models.Employee employee, string roleName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, employee.Email!),
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Role, roleName)
        };

        if (int.TryParse(roleName, out var numericRole))
        {
            claims.Add(new Claim("roleId", numericRole.ToString()));
        }
        else
        {
            if (Enum.TryParse<ProjectManager.Entities.Enums.UserRole>(roleName, out var roleEnum))
            {
                claims.Add(new Claim("roleId", ((int)roleEnum).ToString()));
            }
        }

        var jwtDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        
        var token = tokenHandler.CreateToken(jwtDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}