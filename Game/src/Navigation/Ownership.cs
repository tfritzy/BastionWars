using System.Numerics;
using KeepLordWarriors;

namespace Navigation;

public static class Ownership
{
    public static Dictionary<Vector2Int, ulong> Calculate(int width, int height, Dictionary<ulong, Vector2Int> bastionLocations)
    {
        Dictionary<Vector2Int, ulong> ownership = new();
        Dictionary<ulong, Queue<Vector2Int>> queue = new();
        foreach (ulong id in bastionLocations.Keys)
        {
            queue[id] = new(new Vector2Int[] { bastionLocations[id] });
            ownership[bastionLocations[id]] = id;
        }

        bool allEmpty;
        do
        {
            allEmpty = true;
            foreach (ulong id in queue.Keys)
            {
                Queue<Vector2Int> newItems = new();
                while (queue[id].Count > 0)
                {
                    allEmpty = false;

                    Vector2Int current = queue[id].Dequeue();
                    if (ownership.ContainsKey(current) && ownership[current] != id)
                        continue;
                    ownership[current] = id;
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2Int neighbor = current + Vector2Int.GetDirection(i);
                        if (!IsInBounds(neighbor, width, height))
                        {
                            continue;
                        }

                        if (ownership.ContainsKey(neighbor))
                        {
                            continue;
                        }

                        newItems.Enqueue(neighbor);
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