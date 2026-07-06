namespace ProjectManager.Application.Common;

public interface ICurrentUser
{
    int Id { get; }
    bool IsInRole(string role);
}