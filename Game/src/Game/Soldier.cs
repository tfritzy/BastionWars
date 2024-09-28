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
    public float MovementSpeed { get; private set; }
    public int Health { get; private set; }

    public const float Radius = 0.1f;
    public const float BaseMovementSpeed = 3f;
    public const int BaseHealth = 10;

    public Soldier(Game game, int alliance, SoldierType type, uint source, uint target) : base(game, alliance)
    {
        Type = type;
        SourceKeepId = source;
        TargetKeepId = target;
        MovementSpeed = BaseMovementSpeed;
        Health = BaseHealth;
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
        Vector2 moveDelta = Vector2.Normalize(delta) * MovementSpeed * Game.Time.deltaTime;
        Game.Map.Grid.MoveEntity(Id, currentPos + moveDelta);

        if (Vector2.DistanceSquared(Game.Map.Grid.GetEntityPosition(Id), target.Value) < 0.05f)
        {
            PathProgress++;
        }
    }

    public Vector2 GetVelocity()
    {
        Vector2? target = Game.Map.GetNextPathPoint(SourceKeepId, TargetKeepId, PathProgress);
        if (target == null)
        {
            return Vector2.Zero;
        }

        Vector2 currentPos = Game.Map.Grid.GetEntityPosition(Id);
        Vector2 delta = target.Value - currentPos;
        return Vector2.Normalize(delta) * MovementSpeed;
    }

    public void Freeze()
    {
        MovementSpeed = 0;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Game.Map.RemoveSoldier(Id);
        }
    }
}