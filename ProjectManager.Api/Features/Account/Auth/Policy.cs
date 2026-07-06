namespace ProjectManager.Api.Features.Account.Auth;

public static class Policy
{
    public const string DirectorOnly = "Director";
    public const string DirectorAndManager = "DirectorAndManager";
    public const string All = "All";
}