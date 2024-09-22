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

    public Soldier(Map map, int alliance, SoldierType type, uint source, uint target) : base(map, alliance)
    {
        Type = type;
        SourceKeepId = source;
        TargetKeepId = target;
    }

    public void Update()
    {
        Vector2? target = map.GetNextPathPoint(SourceKeepId, TargetKeepId, PathProgress);
        if (target == null)
        {
            Keep? bastion = map.Keeps[TargetKeepId];
            bastion?.Breach(this);
            map.RemoveSoldier(Id);
            return;
        }
        Vector2 currentPos = map.Grid.GetEntityPosition(Id);
        Vector2 delta = target.Value - currentPos;
        Vector2 moveDelta = Vector2.Normalize(delta) * BaseMovementSpeed * Time.deltaTime;
        map.Grid.MoveEntity(Id, currentPos + moveDelta);

        if (Vector2.DistanceSquared(map.Grid.GetEntityPosition(Id), target.Value) < 0.05f)
        {
            PathProgress++;
        }
    }
}