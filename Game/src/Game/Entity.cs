using SpacialPartitioning;

namespace KeepLordWarriors;

public abstract class Entity
{
    public uint Id { get; private set; }

    /// <summary>
    /// 0 = Neutral, 1+ = Alliance
    /// </summary>
    public int Alliance { get; set; }

    protected Game Game;

    public Entity(Game game, int alliance = 0)
    {
        Id = IdGenerator.NextId();
        Alliance = alliance;
        this.Game = game;
    }
}