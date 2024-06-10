using System.Numerics;

namespace Navigation;

public class NavGrid
{
    private readonly short[,] traversable;

    public NavGrid(short[,] traversable)
    {
        this.traversable = traversable;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        ushort[,] prevMap = InitPrevMap();
        Queue<Vector2Int> q = new();
        HashSet<Vector2Int> visited = new();
        BFS(start, end, q, visited, prevMap);
        return ReconstructPath(start, end, prevMap);
    }

    private void BFS(Vector2Int start, Vector2Int end, Queue<Vector2Int> q, HashSet<Vector2Int> visited, ushort[,] prevMap)
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

            for (int i = 0; i < 8; i++)
            {
                Vector2Int neighbor = current + Vector2Int.GetDirection(i);
                if (!InBounds(neighbor))
                {
                    continue;
                }

                if (visited.Contains(neighbor))
                {
                    continue;
                }

                if (!IsTraversable(neighbor) && neighbor != end)
                {
                    continue;
                }

                prevMap[neighbor.Y, neighbor.X] = GetIndex(current);
                q.Enqueue(neighbor);
                visited.Add(neighbor);
            }
        }
    }

    private bool InBounds(Vector2Int pos)
    {
        return pos.Y >= 0 && pos.Y < traversable.GetLength(0) && pos.X >= 0 && pos.X < traversable.GetLength(1);
    }

    private bool IsTraversable(Vector2Int pos)
    {
        return traversable[pos.Y, pos.X] == 1;
    }

    private List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int end, ushort[,] prevMap)
    {
        List<Vector2Int> path = new();
        Vector2Int current = end;
        while (current != start)
        {
            path.Add(current);
            ushort prev = prevMap[current.Y, current.X];

            if (prev == ushort.MaxValue)
                return new List<Vector2Int>();

            current = GetPosition(prev);
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    private ushort GetIndex(Vector2Int pos)
    {
        return (ushort)(pos.Y * traversable.GetLength(1) + pos.X);
    }

    private Vector2Int GetPosition(ushort index)
    {
        return new Vector2Int(index % traversable.GetLength(1), index / traversable.GetLength(1));
    }

    private ushort[,] InitPrevMap()
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