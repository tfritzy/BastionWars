using System.Numerics;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Soldier : Entity
{
    public SoldierType Type { get; private set; }
    private readonly ulong targetBastionId;
    private readonly ulong sourceBastionId;
    private int pathProgress;

    public const float Radius = 0.5f;
    public const float BaseMovementSpeed = 1.0f;

    public Soldier(Map map, int alliance, SoldierType type, ulong source, ulong target) : base(map, alliance)
    {
        Type = type;
        sourceBastionId = source;
        targetBastionId = target;
    }

    public void Update(float deltaTime)
    {
        Vector2? target = map.GetNextPathPoint(sourceBastionId, targetBastionId, pathProgress);
        if (target == null)
        {
            Bastion? bastion = map.Bastions.Find((b) => b.Id == targetBastionId);
            bastion?.Breach(this);
            map.RemoveSoldier(Id);
            return;
        }
        Vector2 currentPos = map.Grid.GetEntityPosition(Id);
        Vector2 delta = target.Value - currentPos;
        Vector2 moveDelta = Vector2.Normalize(delta) * BaseMovementSpeed * deltaTime;
        map.Grid.MoveEntity(Id, currentPos + moveDelta);

        if (Vector2.DistanceSquared(map.Grid.GetEntityPosition(Id), target.Value) < 0.05f)
        {
            pathProgress++;
        }
    }
}