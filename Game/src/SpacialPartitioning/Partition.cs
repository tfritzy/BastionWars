using System.Numerics;

namespace SpacialPartitioning;

public class Partition
{
    readonly Dictionary<uint, Entity> entities;
    readonly Vector2 bottomLeft;

    public Partition(Vector2 bottomLeft)
    {
        this.bottomLeft = bottomLeft;
        entities = new();
    }

    public Entity GetEntity(uint id)
    {
        return entities[id];
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity.Id, entity);
    }

    public void RemoveEntity(uint id)
    {
        entities.Remove(id);
    }

    public HashSet<uint> GetCollisions(Vector2 point, float radius)
    {
        HashSet<uint> collisions = new();
        foreach (Entity entity in entities.Values)
        {
            if (Vector2.DistanceSquared(point, entity.Position) <= Math.Pow(entity.Radius + radius, 2))
            {
                collisions.Add(entity.Id);
            }
        }

        return collisions;
    }

    public uint? FindCollision(Vector2 point, float radius, Func<uint, bool> predicate)
    {
        foreach (Entity entity in entities.Values)
        {
            if (predicate(entity.Id))
            {
                if (Vector2.DistanceSquared(point, entity.Position) <= Math.Pow(entity.Radius + radius, 2))
                {
                    return entity.Id;
                }
            }
        }

        return null;
    }

    public void UpdateEntityPosition(uint id, Vector2 newPosition)
    {
        entities[id].Position = newPosition;
    }
}