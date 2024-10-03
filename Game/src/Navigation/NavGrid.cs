using System.Numerics;

namespace Navigation;

public static class NavGrid
{
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, short[,] traversable)
    {
        ushort[,] prevMap = InitPrevMap(traversable);
        Queue<Vector2Int> q = new();
        HashSet<Vector2Int> visited = new();
        BFS(start, end, q, visited, prevMap, traversable);
        return ReconstructPath(start, end, prevMap);
    }

    public static ushort[,] GetPathMap(Vector2Int start, short[,] traversable)
    {
        ushort[,] prevMap = InitPrevMap(traversable);
        Queue<Vector2Int> q = new();
        HashSet<Vector2Int> visited = new();
        BFS(start, null, q, visited, prevMap, traversable);
        return prevMap;
    }

    private static void BFS(
        Vector2Int start,
        Vector2Int? end,
        Queue<Vector2Int> q,
        HashSet<Vector2Int> visited,
        ushort[,] prevMap,
        short[,] traversable)
    {
        q.Enqueue(start);
        visited.Add(start);

        while (q.Count > 0)
        {
            Vector2Int current = q.Dequeue();
            if (current == end)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = current + Vector2Int.GetDirection(i);
                if (!InBounds(neighbor, traversable))
                {
                    continue;
                }

                if (visited.Contains(neighbor))
                {
                    continue;
                }

                if (prevMap[neighbor.X, neighbor.Y] == ushort.MaxValue)
                {
                    prevMap[neighbor.X, neighbor.Y] = GetIndex(current, prevMap.GetLength(0));
                }

                if (!IsTraversable(neighbor, traversable))
                {
                    continue;
                }

                q.Enqueue(neighbor);
                visited.Add(neighbor);
            }
        }
    }

    private static bool InBounds(Vector2Int pos, short[,] traversable)
    {
        return pos.Y >= 0 && pos.Y < traversable.GetLength(1) && pos.X >= 0 && pos.X < traversable.GetLength(0);
    }

    private static bool IsTraversable(Vector2Int pos, short[,] traversable)
    {
        return traversable[pos.X, pos.Y] == Constants.TRAVERSABLE;
    }

    public static List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int end, ushort[,] prevMap)
    {
        List<Vector2Int> path = new();
        Vector2Int current = end;
        while (current != start)
        {
            path.Add(current);
            ushort prev = prevMap[current.X, current.Y];

            if (prev == ushort.MaxValue)
                return new List<Vector2Int>();

            current = GetPosition(prev, prevMap.GetLength(0));
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    public static ushort GetIndex(Vector2Int pos, int width)
    {
        return (ushort)(pos.Y * width + pos.X);
    }

    public static Vector2Int GetPosition(ushort index, int width)
    {
        return new Vector2Int(index % width, index / width);
    }

    private static ushort[,] InitPrevMap(short[,] traversable)
    {
        ushort[,] prevMap = new ushort[traversable.GetLength(0), traversable.GetLength(1)];
        for (int x = 0; x < prevMap.GetLength(0); x++)
        {
            for (int y = 0; y < prevMap.GetLength(1); y++)
            {
                prevMap[x, y] = ushort.MaxValue;
            }
        }

        return prevMap;
    }
}