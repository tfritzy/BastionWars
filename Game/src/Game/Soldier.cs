using System.Numerics;
using Schema;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Soldier : Entity
{
    public SoldierType Type { get; private set; }
    public readonly uint TargetKeepId;
    public readonly uint SourceKeepId;
    public int PathIndex { get; set; }
    public float SubPathProgress { get; set; }
    public float MovementSpeed { get; private set; }
    public int Health { get; private set; }
    public float RowOffset { get; private set; }
    public Vector2 Velocity { get; private set; }

    public const float Radius = 0.1f;
    public const float BaseMovementSpeed = .5f;
    public const int BaseHealth = 1;

    public Soldier(Game game, string? owner, int alliance, SoldierType type, uint source, uint target, float rowOffset) : base(game, owner, alliance)
    {
        Type = type;
        SourceKeepId = source;
        TargetKeepId = target;
        MovementSpeed = BaseMovementSpeed;
        Health = BaseHealth;
        RowOffset = rowOffset;
        SubPathProgress = .5f;
    }

    public void Update()
    {
        if (MovementSpeed == 0)
        {
            return;
        }

        List<Vector2Int>? path = Game.Map.GetPathBetweenKeeps(SourceKeepId, TargetKeepId);
        List<WalkPathType>? walkPath = Game.Map.GetWalkPathBetweenKeeps(SourceKeepId, TargetKeepId);
        if (path == null || walkPath == null)
        {
            Game.Map.RemoveSoldier(Id);
            Logger.Log("Error - a soldier's path between keeps was not found and it got deleted");
            return;
        }

        if (PathIndex + 1 >= path.Count)
        {
            Breach();
            return;
        }

        Vector2 prevPos = Pathing.DeterminePathPos(
           path[PathIndex],
           walkPath[PathIndex],
           SubPathProgress
       );

        Pathing.UpdateSoldierPathProgress(this, walkPath, Game.Time.deltaTime);

        if (PathIndex >= 0 && PathIndex < path.Count)
        {
            Vector2 nextPos = Pathing.DeterminePathPos(
                path[PathIndex],
                walkPath[PathIndex],
                SubPathProgress
            );

            Vector2 updatedPos = Pathing.AdjustPosForRowOffset(
             prevPos,
             nextPos,
             RowOffset
            );
            Game.Map.Grid.MoveEntity(Id, updatedPos);
            Velocity = Vector2.Normalize(nextPos - prevPos) * MovementSpeed;
        }
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
            Logger.Log($"Soldier {Id} died at {Game.Map.Grid.GetEntityPosition(Id)}");
            Game.Map.RemoveSoldier(Id);
        }
    }

    private void Breach()
    {
        Keep? bastion = Game.Map.Keeps[TargetKeepId];
        bastion?.Breach(this);
        Game.Map.RemoveSoldier(Id);
    }
}