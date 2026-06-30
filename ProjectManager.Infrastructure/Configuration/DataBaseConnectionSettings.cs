namespace ProjectManager.Infrastructure.Configuration;

public class DataBaseConnectionSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public string ConnectionString => $"Server={Host},{Port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=False;Encrypt=False;Connect Timeout=30;";
}