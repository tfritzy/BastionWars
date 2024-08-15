namespace Helpers;

public struct ResponseDetails<T>
{
    public int StatusCode;
    public T? Body;
}