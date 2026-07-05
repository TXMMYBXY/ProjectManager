namespace ProjectManager.Application.Utils;

public struct Optional<T>
{
    public bool HasValue { get; init; }
    public T Value { get; init; }

    public Optional(T value)
    {
        HasValue = true;
        Value = value;  
    }
}