namespace System.Numerics;

public struct Vector2Int
{
    public int X;
    public int Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }


    public Vector2Int(Schema.V2 v2)
    {
        X = (int)v2.X;
        Y = (int)v2.Y;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2Int operator *(Vector2Int a, int b)
    {
        return new Vector2Int(a.X * b, a.Y * b);
    }

    public static Vector2Int operator /(Vector2Int a, int b)
    {
        return new Vector2Int(a.X / b, a.Y / b);
    }

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int @int && this == @int;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static Vector2Int From(Schema.V2Int vector2)
    {
        return new Vector2Int((int)vector2.X, (int)vector2.Y);
    }

    public static Vector2Int From(Vector2 vector2)
    {
        return new Vector2Int((int)vector2.X, (int)vector2.Y);
    }

    public static Vector2Int Zero => new(0, 0);
    public static Vector2Int One => new(1, 1);
    public static Vector2Int Up => new(0, 1);
    public static Vector2Int TopRight => new(1, 1);
    public static Vector2Int Down => new(0, -1);
    public static Vector2Int TopLeft => new(-1, 1);
    public static Vector2Int Left => new(-1, 0);
    public static Vector2Int BottomRight => new(1, -1);
    public static Vector2Int Right => new(1, 0);
    public static Vector2Int BottomLeft => new(-1, -1);

    public static Vector2Int GetDirection(int i)
    {
        return i switch
        {
            0 => Up,
            1 => Right,
            2 => Down,
            3 => Left,
            4 => TopRight,
            5 => BottomRight,
            6 => BottomLeft,
            7 => TopLeft,
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    public override string ToString()
    {
        return $"<{X}, {Y}>";
    }
}