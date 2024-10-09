using Schema;

namespace System.Numerics
{
    public static class Vector3Extensions
    {
        public static V3 ToSchema(this Vector3 vector)
        {
            return new V3() { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        public static V2 ToSchema(this Vector2 vector)
        {
            return new V2() { X = vector.X, Y = vector.Y };
        }

        public static V2Int ToSchema(this Vector2Int vector)
        {
            return new V2Int() { X = vector.X, Y = vector.Y };
        }

        public static Vector2Int FromSchema(V2Int vector)
        {
            return new Vector2Int() { X = vector.X, Y = vector.Y };
        }

        public static Vector2 Add(this Vector2 vector, float val)
        {
            vector.X += val;
            vector.Y += val;
            return vector;
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y); // Discard Z component
        }
    }
}
