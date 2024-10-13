using SpacialPartitioning;

namespace KeepLordWarriors;

public abstract class Entity
{
    public uint Id { get; private set; }

    public string? OwnerId { get; set; }
    public int Alliance { get; set; }

    protected Game Game;

    public Entity(Game game, string? ownerId, int alliance = 0)
    {
        Id = IdGenerator.NextId();
        OwnerId = ownerId;
        Alliance = alliance;
        this.Game = game;
    }
}