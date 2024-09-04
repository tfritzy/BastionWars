namespace KeepLordWarriors;

public static class IdGenerator
{
    static uint nextId = 1;

    public static uint NextId()
    {
        return nextId++;
    }
}