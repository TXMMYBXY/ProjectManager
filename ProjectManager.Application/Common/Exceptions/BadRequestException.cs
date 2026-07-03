namespace ProjectManager.Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }

    public static void ThrowIfError(string message)
    {
        throw new BadRequestException(message);
    }
}