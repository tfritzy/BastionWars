using System.Numerics;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Soldier : Entity
{
    public SoldierType Type { get; private set; }
    public readonly uint TargetBastionId;
    public readonly uint SourceBastionId;
    public int PathProgress { get; private set; }

    public const float Radius = 0.5f;
    public const float BaseMovementSpeed = 1.0f;

    public Soldier(Map map, int alliance, SoldierType type, uint source, uint target) : base(map, alliance)
    {
        Type = type;
        SourceBastionId = source;
        TargetBastionId = target;
    }

    public void Update(double deltaTime)
    {
        Vector2? target = map.GetNextPathPoint(SourceBastionId, TargetBastionId, PathProgress);
        if (target == null)
        {
            Keep? bastion = map.Keeps[TargetBastionId];
            bastion?.Breach(this);
            map.RemoveSoldier(Id);
            return;
        }
        Vector2 currentPos = map.Grid.GetEntityPosition(Id);
        Vector2 delta = target.Value - currentPos;
        Vector2 moveDelta = Vector2.Normalize(delta) * BaseMovementSpeed * (float)deltaTime;
        map.Grid.MoveEntity(Id, currentPos + moveDelta);

        if (Vector2.DistanceSquared(map.Grid.GetEntityPosition(Id), target.Value) < 0.05f)
        {
            PathProgress++;
        }
    }
}