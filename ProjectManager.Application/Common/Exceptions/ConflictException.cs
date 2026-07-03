namespace ProjectManager.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
    
    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ConflictException(message);
    }
}