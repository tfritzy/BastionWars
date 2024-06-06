namespace BastionWars;

public static class IdGenerator
{
    static ulong nextId = 1;

    public static ulong NextId()
    {
        return nextId++;
    }
}