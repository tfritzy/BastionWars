using System.Numerics;

namespace SpacialPartitioning;

public class Grid
{
    public int EntityCount => entityPartitionLookup.Count;
    public const int PartitionSize = 5;

    readonly Partition[,] partitions;
    readonly Dictionary<ulong, V2Int> entityPartitionLookup;

    public Grid(int sizeX, int sizeY)
    {
        if (sizeX % PartitionSize != 0 || sizeY % PartitionSize != 0)
        {
            throw new ArgumentException("Size must be divisible by partition size");
        }

        partitions = new Partition[sizeX / PartitionSize, sizeY / PartitionSize];
        entityPartitionLookup = new();
        for (int x = 0; x < partitions.GetLength(0); x++)
        {
            for (int y = 0; y < partitions.GetLength(1); y++)
            {
                partitions[x, y] = new Partition(new Vector2(x * PartitionSize, y * PartitionSize));
            }
        }
    }

    public void AddEntity(Entity entity)
    {
        int x = (int)(entity.Position.X / PartitionSize);
        int y = (int)(entity.Position.Y / PartitionSize);
        partitions[x, y].AddEntity(entity);

        entityPartitionLookup.Add(entity.Id, new V2Int(x, y));
    }

    public void RemoveEntity(ulong id)
    {
        V2Int index = entityPartitionLookup[id];
        partitions[index.X, index.Y].RemoveEntity(id);
        entityPartitionLookup.Remove(id);
    }

    public void MoveEntity(ulong id, Vector2 newPosition)
    {
        V2Int newPartition = new(
            (int)(newPosition.X / PartitionSize),
            (int)(newPosition.Y / PartitionSize));

        if (!entityPartitionLookup.ContainsKey(id))
        {
            throw new ArgumentException("Entity not found");
        }

        if (newPartition.X < 0 || newPartition.X >= partitions.GetLength(0) ||
            newPartition.Y < 0 || newPartition.Y >= partitions.GetLength(1))
        {
            throw new ArgumentException("Entity out of bounds");
        }

        if (entityPartitionLookup[id] == newPartition)
        {
            partitions[newPartition.X, newPartition.Y].UpdateEntityPosition(id, newPosition);
        }
        else
        {
            Partition partition = partitions[entityPartitionLookup[id].X, entityPartitionLookup[id].Y];
            Entity entity = partition.GetEntity(id);
            partitions[entityPartitionLookup[id].X, entityPartitionLookup[id].Y].RemoveEntity(id);
            entity.Position = newPosition;
            partitions[newPartition.X, newPartition.Y].AddEntity(entity);
            entityPartitionLookup[id] = newPartition;
        }
    }

    public List<ulong> GetCollisions(Vector2 point, float radius)
    {
        int xPartMin = Math.Max((int)((point.X - radius) / PartitionSize - 1), 0);
        int xPartMax = Math.Min((int)((point.X + radius) / PartitionSize) + 1, partitions.GetLength(0) - 1);
        int yPartMin = Math.Max((int)((point.Y - radius) / PartitionSize) - 1, 0);
        int yPartMax = Math.Min((int)((point.Y + radius) / PartitionSize) + 1, partitions.GetLength(1) - 1);

        List<ulong> collisions = new();
        for (int x = xPartMin; x <= xPartMax; x++)
        {
            for (int y = yPartMin; y <= yPartMax; y++)
            {
                collisions.AddRange(partitions[x, y].GetCollisions(point, radius));
            }
        }

        return collisions;
    }

    public bool ContainsEntity(ulong id)
    {
        return entityPartitionLookup.ContainsKey(id);
    }

    public Vector2 GetEntityPosition(ulong id)
    {
        if (!entityPartitionLookup.ContainsKey(id))
        {
            throw new ArgumentException("Entity not found");
        }

        return GetPartition(id).GetEntity(id).Position;
    }

    public Schema.Vector2 GetEntitySchemaPosition(ulong id)
    {
        if (!entityPartitionLookup.ContainsKey(id))
        {
            throw new ArgumentException("Entity not found");
        }

        var pos = GetPartition(id).GetEntity(id).Position;

        return new Schema.Vector2
        {
            X = pos.X,
            Y = pos.Y
        };
    }

    public V2Int? GetEntityGridPos(ulong id)
    {
        if (!entityPartitionLookup.ContainsKey(id))
        {
            return null;
        }

        return V2Int.From(GetPartition(id).GetEntity(id).Position);
    }

    private Partition GetPartition(ulong id)
    {
        if (!entityPartitionLookup.ContainsKey(id))
        {
            throw new ArgumentException("Entity not found");
        }

        return partitions[
            entityPartitionLookup[id].X,
            entityPartitionLookup[id].Y];
    }
}
