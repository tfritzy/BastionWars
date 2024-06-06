using System.Numerics;

namespace SpacialPartitioning;

public class Grid
{
    public const int PartitionSize = 5;

    readonly Partition[,] partitions;

    public Grid(int sizeX, int sizeY)
    {
        if (sizeX % PartitionSize != 0 || sizeY % PartitionSize != 0)
        {
            throw new ArgumentException("Size must be divisible by partition size");
        }

        partitions = new Partition[sizeX / PartitionSize, sizeY / PartitionSize];
        for (int x = 0; x < partitions.GetLength(0); x++)
        {
            for (int y = 0; y < partitions.GetLength(1); y++)
            {
                partitions[x, y] = new Partition();
            }
        }
    }

    public void AddEntity(Entity entity)
    {
        int x = (int)(entity.Position.X / PartitionSize);
        int y = (int)(entity.Position.Y / PartitionSize);
        partitions[x, y].AddEntity(entity);
    }

    public List<ulong> GetCollisions(Vector2 point, float radius)
    {
        int xPartMin = Math.Max((int)((point.X - radius) / PartitionSize), 0);
        int xPartMax = Math.Min((int)((point.X + radius) / PartitionSize), partitions.GetLength(0) - 1);
        int yPartMin = Math.Max((int)((point.Y - radius) / PartitionSize), 0);
        int yPartMax = Math.Min((int)((point.Y + radius) / PartitionSize), partitions.GetLength(1) - 1);

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
}
