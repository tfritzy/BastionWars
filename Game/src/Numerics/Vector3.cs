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

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y); // Discard Z component
        }
    }
}