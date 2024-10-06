using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Numerics;
using KeepLordWarriors;
using Schema;

public class Pathing
{

    private const float cornerTurnRadius = 0.5f;

    public static float GetPathLength(WalkPathType walkType)
    {
        if (
         walkType == WalkPathType.StraightDown ||
         walkType == WalkPathType.StraightToRight ||
         walkType == WalkPathType.StraightUp ||
         walkType == WalkPathType.StraightLeft
        )
        {
            return 1;
        }
        else
        {
            return 0.7855f;
        }
    }

    const float A_270 = MathF.PI + MathF.PI / 2;
    const float A_180 = MathF.PI;
    const float A_90 = MathF.PI / 2;
    const float A_0 = 0;

    public static Vector2 DeterminePathPos(
     Vector2Int p,
     WalkPathType walkType,
     float distanceAlong)
    {
        switch (walkType)
        {
            case WalkPathType.StraightToRight:
                return new Vector2(x: p.X + distanceAlong, y: p.Y + 0.5f);
            case WalkPathType.StraightDown:
                return new Vector2(x: p.X + 0.5f, y: p.Y + distanceAlong);
            case WalkPathType.StraightUp:
                return new Vector2(x: p.X + 0.5f, y: p.Y + 1 - distanceAlong);
            case WalkPathType.StraightLeft:
                return new Vector2(x: p.X + 1 - distanceAlong, y: p.Y + 0.5f);
            case WalkPathType.CircularLeftDown:
                return DetermineCircularPos(p.X, p.Y + 1, A_90, true, distanceAlong);
            case WalkPathType.CircularLeftUp:
                return DetermineCircularPos(p.X, p.Y, A_270, false, distanceAlong);
            case WalkPathType.CircularDownLeft:
                return DetermineCircularPos(p.X, p.Y + 1, A_0, false, distanceAlong);
            case WalkPathType.CircularDownRight:
                return DetermineCircularPos(p.X + 1, p.Y + 1, A_180, true, distanceAlong);
            case WalkPathType.CircularRightDown:
                return DetermineCircularPos(p.X + 1, p.Y + 1, A_90, false, distanceAlong);
            case WalkPathType.CircularRightUp:
                return DetermineCircularPos(p.X + 1, p.Y, A_270, true, distanceAlong);
            case WalkPathType.CircularUpRight:
                return DetermineCircularPos(p.X + 1, p.Y, A_180, false, distanceAlong);
            case WalkPathType.CircularUpLeft:
                return DetermineCircularPos(p.X, p.Y, A_0, true, distanceAlong);
            default:
                return Vector2.Zero;
        }
    }


    public static Vector2 DetermineCircularPos(
      float circleCenterX,
      float circleCenterY,
      float startAngle,
      Boolean clockwise,
      float distanceAlong
    )
    {
        float angleTraversed = (distanceAlong / (0.5f * 2 * MathF.PI)) * (2 * MathF.PI);

        float finalAngle = clockwise
          ? startAngle - angleTraversed
          : startAngle + angleTraversed;

        finalAngle = finalAngle % (2 * MathF.PI);
        if (finalAngle < 0) finalAngle += 2 * MathF.PI;

        float x = circleCenterX + 0.5f * MathF.Cos(finalAngle);
        float y = circleCenterY - 0.5f * MathF.Sin(finalAngle);

        return new Vector2(x, y);
    }

    public static void UpdateSoldierPathProgress(
        Soldier soldier,
        List<WalkPathType> walkPath,
        float deltaTime
    )
    {
        if (soldier.PathIndex < 0 || soldier.PathIndex >= walkPath.Count)
        {
            return;
        }

        soldier.SubPathProgress += soldier.MovementSpeed * deltaTime;
        float segmentLength = GetPathLength(walkPath[soldier.PathIndex]);
        if (soldier.SubPathProgress >= segmentLength)
        {
            soldier.PathIndex += 1;
            soldier.SubPathProgress -= segmentLength;
        }
    }

    public static Vector2 AdjustPosForRowOffset(
      Vector2 currentPos,
      Vector2 nextPos,
      float rowOffset
    )
    {
        Vector2 perp = Vector2.Normalize(new Vector2(
            x: nextPos.Y - currentPos.Y,
            y: -(nextPos.X - currentPos.X)
        ));

        perp.X = nextPos.X + perp.X * rowOffset;
        perp.Y = nextPos.Y + perp.Y * rowOffset;

        return perp;
    }

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
}