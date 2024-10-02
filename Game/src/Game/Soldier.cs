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
    public float SubPathProgress { get; private set; }
    public float MovementSpeed { get; private set; }
    public int Health { get; private set; }
    public float RowOffset { get; private set; }

    public const float Radius = 0.1f;
    public const float BaseMovementSpeed = 3f;
    public const int BaseHealth = 10;

    public Soldier(Game game, int alliance, SoldierType type, uint source, uint target, float rowOffset) : base(game, alliance)
    {
        Type = type;
        SourceKeepId = source;
        TargetKeepId = target;
        MovementSpeed = BaseMovementSpeed;
        Health = BaseHealth;
        RowOffset = rowOffset;
    }

    public void Update()
    {
        if (MovementSpeed == 0)
        {
            return;
        }

        List<Vector2Int>? path = Game.Map.GetPathBetweenKeeps(SourceKeepId, TargetKeepId);
        if (path == null)
        {
            Game.Map.RemoveSoldier(Id);
            Logger.Log("Error - a soldier's path between keeps was not found and it got deleted");
            return;
        }

        if (PathProgress + 1 >= path.Count)
        {
            Breach();
            return;
        }

        Vector2Int currentTile = path[PathProgress];
        Vector2Int nextTile = path[PathProgress + 1];
        float movementDelta = MovementSpeed * Game.Time.deltaTime;
        SubPathProgress += movementDelta;

        Pathing.PathType type = Pathing.DeterminePathType(currentTile, nextTile);
        float subPathLength = Pathing.PathLengths[type];

        if (SubPathProgress >= subPathLength)
        {
            PathProgress += 1;
            SubPathProgress = SubPathProgress - subPathLength;

            // This is a little incorrect. Ideally the entity
            // position would be updated to match the sliver of
            // sub path progress we will have here, but since
            // the server's position doesn't get directly rendered
            // I feel it's not worth the complexity of handling it here.
            return;
        }

        Vector2 newPos = Pathing.DeterminePathPos(currentTile, nextTile, SubPathProgress);
        Game.Map.Grid.MoveEntity(Id, newPos);
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

    private void Breach()
    {
        Keep? bastion = Game.Map.Keeps[TargetKeepId];
        bastion?.Breach(this);
        Game.Map.RemoveSoldier(Id);
    }
}