using System.Numerics;
using KeepLordWarriors;

namespace Navigation;

public static class Ownership
{
    public static uint[,] Calculate(
        int width,
        int height,
        Dictionary<uint, Vector2Int> bastionLocations)
    {
        uint[,] ownership = new uint[width, height];
        Dictionary<uint, Queue<Vector2Int>> queue = new();
        foreach (uint id in bastionLocations.Keys)
        {
            queue[id] = new(new Vector2Int[] { bastionLocations[id] });
            ownership[bastionLocations[id].X, bastionLocations[id].Y] = id;
        }

        bool allEmpty;
        do
        {
            allEmpty = true;
            foreach (uint id in queue.Keys)
            {
                Queue<Vector2Int> newItems = new();
                while (queue[id].Count > 0)
                {
                    allEmpty = false;

                    Vector2Int current = queue[id].Dequeue();
                    if (ownership[current.X, current.Y] != 0 && ownership[current.X, current.Y] != id)
                        continue;
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2Int neighbor = current + Vector2Int.GetDirection(i);
                        if (!IsInBounds(neighbor, width, height))
                        {
                            continue;
                        }

                        if (ownership[neighbor.X, neighbor.Y] != 0)
                        {
                            continue;
                        }

                        newItems.Enqueue(neighbor);
                        ownership[neighbor.X, neighbor.Y] = id;
                    }
                }
                queue[id] = newItems;
            }
        }
        while (!allEmpty);

        return ownership;
    }

    private static bool IsInBounds(Vector2Int pos, int width, int height)
    {
        return pos.Y >= 0 && pos.Y < height && pos.X >= 0 && pos.X < width;
    }
}