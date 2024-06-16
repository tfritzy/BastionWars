using System.Numerics;

namespace Navigation;

public static class NavGrid
{
    public static List<V2Int> FindPath(V2Int start, V2Int end, short[,] traversable)
    {
        ushort[,] prevMap = InitPrevMap(traversable);
        Queue<V2Int> q = new();
        HashSet<V2Int> visited = new();
        BFS(start, end, q, visited, prevMap, traversable);
        return ReconstructPath(start, end, prevMap);
    }

    public static ushort[,] GetPathMap(V2Int start, short[,] traversable)
    {
        ushort[,] prevMap = InitPrevMap(traversable);
        Queue<V2Int> q = new();
        HashSet<V2Int> visited = new();
        BFS(start, null, q, visited, prevMap, traversable);
        return prevMap;
    }

    private static void BFS(
        V2Int start,
        V2Int? end,
        Queue<V2Int> q,
        HashSet<V2Int> visited,
        ushort[,] prevMap,
        short[,] traversable)
    {
        q.Enqueue(start);
        visited.Add(start);

        while (q.Count > 0)
        {
            V2Int current = q.Dequeue();
            if (current == end)
            {
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                V2Int neighbor = current + V2Int.GetDirection(i);
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

    private static bool InBounds(V2Int pos, short[,] traversable)
    {
        return pos.Y >= 0 && pos.Y < traversable.GetLength(1) && pos.X >= 0 && pos.X < traversable.GetLength(0);
    }

    private static bool IsTraversable(V2Int pos, short[,] traversable)
    {
        return traversable[pos.X, pos.Y] == 1;
    }

    public static List<V2Int> ReconstructPath(V2Int start, V2Int end, ushort[,] prevMap)
    {
        List<V2Int> path = new();
        V2Int current = end;
        while (current != start)
        {
            path.Add(current);
            ushort prev = prevMap[current.X, current.Y];

            if (prev == ushort.MaxValue)
                return new List<V2Int>();

            current = GetPosition(prev, prevMap.GetLength(0));
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    public static ushort GetIndex(V2Int pos, int width)
    {
        return (ushort)(pos.Y * width + pos.X);
    }

    public static V2Int GetPosition(ushort index, int width)
    {
        return new V2Int(index % width, index / width);
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