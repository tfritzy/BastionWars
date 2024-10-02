using System;
using System.Collections.Generic;
using System.Numerics;

public class Pathing
{
    public enum PathType
    {
        Straight,
        Diagonal,
        Circular,
        StraightDiagonal
    }

    private static readonly Dictionary<PathType, float> PathLengths = new Dictionary<PathType, float>
    {
        { PathType.Straight, 1.0f },
        { PathType.Diagonal, 1.414f }, // Approximately sqrt(2)
        { PathType.Circular, 1.571f }, // Approximately pi/2
        { PathType.StraightDiagonal, 1.5f } // Approximation for straight + diagonal
    };

    public static float CalculatePathLength(List<Vector2Int> stops)
    {
        if (stops == null || stops.Count < 2)
            return 0f;

        float totalLength = 0f;

        for (int i = 0; i < stops.Count - 1; i++)
        {
            Vector2Int current = stops[i];
            Vector2Int next = stops[i + 1];

            PathType pathType = DeterminePathType(current, next);
            totalLength += PathLengths[pathType];
        }

        return totalLength;
    }

    private static PathType DeterminePathType(Vector2Int from, Vector2Int to)
    {
        int dx = Math.Abs(to.X - from.X);
        int dy = Math.Abs(to.Y - from.Y);

        if (dx == 0 && dy == 0)
            return PathType.Straight; // No movement

        if (dx == 1 && dy == 0 || dx == 0 && dy == 1)
            return PathType.Straight;

        if (dx == 1 && dy == 1)
            return PathType.Diagonal;

        if (dx + dy == 1)
            return PathType.Circular;

        if (dx + dy == 2 && (dx == 1 || dy == 1))
            return PathType.StraightDiagonal;

        throw new ArgumentException("Invalid path segment: movement greater than one tile is not supported.");
    }
}