namespace ProjectManager.Api.Utils;

public struct Optional<T>
{
    public bool HasValue { get; }
    public T Value { get; }

    public Optional(T value)
    {
        HasValue = true;
        Value = value;  
    }
}