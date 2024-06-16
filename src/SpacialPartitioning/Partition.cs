using System.Numerics;

namespace SpacialPartitioning;

public class Partition
{
    readonly Dictionary<ulong, Entity> entities;
    readonly Vector2 bottomLeft;

    public Partition(Vector2 bottomLeft)
    {
        this.bottomLeft = bottomLeft;
        entities = new();
    }

    public Entity GetEntity(ulong id)
    {
        return entities[id];
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity.Id, entity);
    }

    public void RemoveEntity(ulong id)
    {
        entities.Remove(id);
    }

    public HashSet<ulong> GetCollisions(Vector2 point, float radius)
    {
        HashSet<ulong> collisions = new();
        foreach (Entity entity in entities.Values)
        {
            if (Vector2.DistanceSquared(point, entity.Position) <= Math.Pow(entity.Radius + radius, 2))
            {
                collisions.Add(entity.Id);
            }
        }

        return collisions;
    }

    public void UpdateEntityPosition(ulong id, Vector2 newPosition)
    {
        entities[id].Position = newPosition;
    }
}