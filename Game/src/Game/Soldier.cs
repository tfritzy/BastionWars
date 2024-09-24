using System.Numerics;
using Schema;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Soldier : Entity
{
    public SoldierType Type { get; private set; }
    public readonly uint TargetKeepId;
    public readonly uint SourceKeepId;
    public int PathProgress { get; private set; }

    public const float Radius = 0.5f;
    public const float BaseMovementSpeed = 1.0f;

    public Soldier(Game game, int alliance, SoldierType type, uint source, uint target) : base(game, alliance)
    {
        Type = type;
        SourceKeepId = source;
        TargetKeepId = target;
    }

    public void Update()
    {
        Vector2? target = Game.Map.GetNextPathPoint(SourceKeepId, TargetKeepId, PathProgress);
        if (target == null)
        {
            Keep? bastion = Game.Map.Keeps[TargetKeepId];
            bastion?.Breach(this);
            Game.Map.RemoveSoldier(Id);
            return;
        }
        Vector2 currentPos = Game.Map.Grid.GetEntityPosition(Id);
        Vector2 delta = target.Value - currentPos;
        Vector2 moveDelta = Vector2.Normalize(delta) * BaseMovementSpeed * Game.Time.deltaTime;
        Game.Map.Grid.MoveEntity(Id, currentPos + moveDelta);

        if (Vector2.DistanceSquared(Game.Map.Grid.GetEntityPosition(Id), target.Value) < 0.05f)
        {
            PathProgress++;
        }
    }
}