namespace BastionWars;

public class Grid
{
    public const int PartitionSize = 10;

    Partition[,] partitions;

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
        int x = (int)(entity.x / PartitionSize);
        int y = (int)(entity.y / PartitionSize);
        partitions[x, y].AddEntity(entity);
    }
}
