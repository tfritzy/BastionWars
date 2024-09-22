namespace KeepLordWarriors;

public static class Time
{
    public static float Now;
    public static float deltaTime;

    public static void Update(float now)
    {
        deltaTime = now - Now;
        Now = now;
    }
}