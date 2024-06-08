namespace BastionWars;

public abstract class Entity
{
    public ulong Id;

    /// <summary>
    /// 0 = Neutral, 1+ = Alliance
    /// </summary>
    public int Alliance;

    public Entity(int alliance = 0)
    {
        Id = IdGenerator.NextId();
        Alliance = alliance;
    }
}