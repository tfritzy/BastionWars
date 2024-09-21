using SpacialPartitioning;

namespace KeepLordWarriors;

public abstract class Entity
{
    public uint Id { get; private set; }

    /// <summary>
    /// 0 = Neutral, 1+ = Alliance
    /// </summary>
    public int Alliance { get; set; }

    protected Map map;

    public Entity(Map map, int alliance = 0)
    {
        Id = IdGenerator.NextId();
        Alliance = alliance;
        this.map = map;
    }
}