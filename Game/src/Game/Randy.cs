namespace KeepLordWarriors;

public static class Randy
{
    public static System.Random WorldGen = new();
    public static System.Random Chaos = new();

    public static void SetSeed(int seed)
    {
        WorldGen = new System.Random(seed);
    }

    public static float ChaoticInRange(float min, float max)
    {
        return (float)(Chaos.NextDouble() * (max - min) + min);
    }

    public static T ChaoticElement<T>(List<T> list)
    {
        return list[Chaos.Next(0, list.Count)];
    }
}