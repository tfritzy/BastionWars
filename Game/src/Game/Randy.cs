namespace KeepLordWarriors;

public class Randy
{
    public Random WorldGen = new();
    public Random Seeded = new();
    public Random Chaos = new();

    public Randy(int seed)
    {
        WorldGen = seed > 0 ? new Random(seed) : new Random();
        Seeded = seed > 0 ? new Random(seed) : new Random();
    }

    public float ChaoticInRange(float min, float max)
    {
        return (float)(Chaos.NextDouble() * (max - min) + min);
    }

    public T ChaoticElement<T>(IList<T> list)
    {
        return list[Chaos.Next(0, list.Count)];
    }

    public float SeededInRange(float min, float max)
    {
        return (float)(Chaos.NextDouble() * (max - min) + min);
    }

    public T SeededElement<T>(IList<T> list)
    {
        return list[Chaos.Next(0, list.Count)];
    }
}