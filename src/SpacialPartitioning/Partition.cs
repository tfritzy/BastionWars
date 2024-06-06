using System.Numerics;

namespace SpacialPartitioning;

public class Partition
{
    readonly List<Entity> entities;

    public Partition()
    {
        entities = new();
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }

    public List<ulong> GetCollisions(Vector2 point, float radius)
    {
        List<ulong> collisions = new();
        foreach (Entity entity in entities)
        {
            if (Vector2.DistanceSquared(point, entity.Position) <= Math.Pow(entity.Radius + radius, 2))
            {
                collisions.Add(entity.Id);
            }
        }

        return collisions;
    }
}