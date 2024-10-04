using System;
using System.Collections.Generic;
using System.Numerics;
using Schema;

public class Pathing
{
    public enum PathType
    {
        Straight,
        Diagonal,
        Circular,
        CornerTurn
    }

    private const float cornerTurnRadius = 0.5f;

    public static readonly Dictionary<PathType, float> PathLengths = new Dictionary<PathType, float>
    {
        { PathType.Straight, 1.0f },
        { PathType.Diagonal, 1.414f }, // Approximately sqrt(2)
        { PathType.Circular, 1.571f }, // Approximately pi/2
        { PathType.CornerTurn, 1f - cornerTurnRadius + 0.5f * MathF.PI * cornerTurnRadius } // Precise calculation
    };

    public static List<WalkPathType> GetWalkTypes(List<Vector2Int> path)
    {
        List<WalkPathType> walkPath = new(path.Count);

        for (int pi = 0; pi < path.Count; pi++)
        {
            if (pi == 0)
            {
                walkPath.Add(GetWalkPathType(null, path[0], path[1]));
            }
            else if (pi < path.Count - 1)
            {
                walkPath.Add(GetWalkPathType(path[pi - 1], path[pi], path[pi + 1]));
            }
            else
            {
                walkPath.Add(GetWalkPathType(path[pi - 1], path[pi], null));
            }
        }

        return walkPath;
    }

    private static WalkPathType GetWalkPathType(Vector2Int? prev, Vector2Int cur, Vector2Int? next)
    {
        if (prev == null)
        {
            return GetStraightDir(cur, next!.Value);
        }

        if (next == null)
        {
            return GetStraightDir(prev!.Value, cur);
        }

        var prevDir = cur - prev!.Value;
        var nextDir = next!.Value - cur;

        if (prevDir.X != 0 && prevDir.X != nextDir.X)
        {
            // circular
            if (prevDir.X > 0 && nextDir.Y > 0)
            {
                return WalkPathType.CircularLeftDown;
            }
            else if (prevDir.X > 0 && nextDir.Y < 0)
            {
                return WalkPathType.CircularLeftUp;
            }
            else if (prevDir.X < 0 && nextDir.Y > 0)
            {
                return WalkPathType.CircularRightDown;
            }
            else if (prevDir.X < 0 && nextDir.Y < 0)
            {
                return WalkPathType.CircularRightUp;
            }
            else
            {
                throw new Exception($"Unexpected circular condition prev:{prevDir} next:{nextDir}");
            }
        }
        else if (prevDir.Y != 0 && prevDir.Y != nextDir.Y)
        {
            // circular
            if (prevDir.Y > 0 && nextDir.X < 0)
            {
                return WalkPathType.CircularUpLeft;
            }
            else if (prevDir.Y > 0 && nextDir.X > 0)
            {
                return WalkPathType.CircularUpRight;
            }
            else if (prevDir.Y < 0 && nextDir.X < 0)
            {
                return WalkPathType.CircularDownLeft;
            }
            else if (prevDir.Y < 0 && nextDir.X > 0)
            {
                return WalkPathType.CircularDownRight;
            }
            else
            {
                throw new Exception($"Unexpected circular condition prev:{prevDir} next:{nextDir}");
            }
        }
        else
        {
            return GetStraightDir(cur, next!.Value);
        }
    }

    private static WalkPathType GetStraightDir(Vector2Int cur, Vector2Int next)
    {
        var dir = next - cur;
        if (dir.X > 0)
        {
            return WalkPathType.StraightToRight;
        }
        else if (dir.X < 0)
        {
            return WalkPathType.StraightLeft;
        }
        else if (dir.Y > 0)
        {
            return WalkPathType.StraightDown;
        }
        else
        {
            return WalkPathType.StraightUp;
        }
    }

    public static PathType DeterminePathType(Vector2Int from, Vector2Int to)
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
            return PathType.CornerTurn;

        throw new ArgumentException("Invalid path segment: movement greater than one tile is not supported.");
    }

    public static bool HasFinishedSubPath(Vector2Int source, Vector2Int target, float distanceAlong)
    {
        PathType type = DeterminePathType(source, target);
        float distance = PathLengths[type];
        return distanceAlong >= distance;
    }

    public static Vector2 DeterminePathPos(Vector2Int source, Vector2Int target, float distanceAlong)
    {
        PathType type = DeterminePathType(source, target);
        Vector2 direction = Vector2.Normalize(new Vector2(target.X - source.X, target.Y - source.Y));
        float normalizedDistance = Math.Clamp(distanceAlong / PathLengths[type], 0f, 1f);
        Vector2 basePosition = new Vector2(source.X + 0.5f, source.Y + 0.5f);  // Center of the source tile

        switch (type)
        {
            case PathType.Straight:
            case PathType.Diagonal:
                return basePosition + direction * normalizedDistance;

            case PathType.Circular:
                return CalculateCircularPosition(basePosition, direction, normalizedDistance);

            case PathType.CornerTurn:
                return CalculateCornerTurnPosition(basePosition, direction, normalizedDistance);

            default:
                throw new ArgumentException("Invalid path type");
        }
    }

    private static Vector2 CalculateCircularPosition(Vector2 basePosition, Vector2 direction, float normalizedDistance)
    {
        float angle = normalizedDistance * MathF.PI / 2;
        Vector2 rotatedDirection = new Vector2(
            direction.X * MathF.Cos(angle) - direction.Y * MathF.Sin(angle),
            direction.X * MathF.Sin(angle) + direction.Y * MathF.Cos(angle)
        );
        return basePosition + rotatedDirection;
    }

    private static Vector2 CalculateCornerTurnPosition(Vector2 basePosition, Vector2 direction, float normalizedDistance)
    {
        float straightPart = 1f - cornerTurnRadius;
        float arcLength = 0.5f * MathF.PI * cornerTurnRadius;
        float totalLength = straightPart + arcLength;

        if (normalizedDistance <= straightPart / totalLength)
        {
            // On the straight part
            return basePosition + direction * (normalizedDistance * totalLength);
        }
        else
        {
            // On the curved part
            float arcDistance = (normalizedDistance * totalLength) - straightPart;
            float angle = arcDistance / cornerTurnRadius;
            Vector2 cornerCenter = basePosition + direction * straightPart;
            Vector2 perpDirection = new Vector2(-direction.Y, direction.X);
            return cornerCenter + (direction * cornerTurnRadius * MathF.Sin(angle)) +
                   (perpDirection * cornerTurnRadius * (1 - MathF.Cos(angle)));
        }
    }
}