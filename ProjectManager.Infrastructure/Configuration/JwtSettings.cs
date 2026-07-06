namespace ProjectManager.Infrastructure.Configuration;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiresMinutes { get; set; }
}