namespace System.Numerics;

public struct V2Int
{
    public int X;
    public int Y;

    public V2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static V2Int operator +(V2Int a, V2Int b)
    {
        return new V2Int(a.X + b.X, a.Y + b.Y);
    }

    public static V2Int operator -(V2Int a, V2Int b)
    {
        return new V2Int(a.X - b.X, a.Y - b.Y);
    }

    public static V2Int operator *(V2Int a, int b)
    {
        return new V2Int(a.X * b, a.Y * b);
    }

    public static V2Int operator /(V2Int a, int b)
    {
        return new V2Int(a.X / b, a.Y / b);
    }

    public static bool operator ==(V2Int a, V2Int b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(V2Int a, V2Int b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is V2Int @int && this == @int;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static V2Int From(Vector2 vector2)
    {
        return new V2Int((int)vector2.X, (int)vector2.Y);
    }

    public static V2Int Zero => new(0, 0);
    public static V2Int One => new(1, 1);
    public static V2Int Up => new(0, 1);
    public static V2Int TopRight => new(1, 1);
    public static V2Int Down => new(0, -1);
    public static V2Int TopLeft => new(-1, 1);
    public static V2Int Left => new(-1, 0);
    public static V2Int BottomRight => new(1, -1);
    public static V2Int Right => new(1, 0);
    public static V2Int BottomLeft => new(-1, -1);

    public static V2Int GetDirection(int i)
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