namespace ProjectManager.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    
    public static void ThrowIfNull(object? value, string message)
    {
        if (value is null)
            throw new NotFoundException(message);
    }
}