using System;
using System.Numerics;

namespace KeepLordWarriors;

public class Projectile(Vector3 startPos, float birthTime, Vector3 initialVelocity)
{
    public Vector3 StartPos { get; private set; } = startPos;
    public float BirthTime { get; private set; } = birthTime;
    public Vector3 InitialVelocity { get; private set; } = initialVelocity;
    public float TimeWillLand { get; private set; } = birthTime + GetFlightDuration(initialVelocity);
    public Vector3 FinalPosition { get; private set; } = CalculateFinalPosition(startPos, initialVelocity);

    private static float GetFlightDuration(Vector3 initialVelocity)
    {
        float timeToPeak = Math.Abs(initialVelocity.Z) / Constants.ArrowGravity;
        return timeToPeak * 2;
    }

    private static Vector3 CalculateFinalPosition(Vector3 startPos, Vector3 initialVelocity)
    {
        float flightDuration = GetFlightDuration(initialVelocity);
        Vector3 finalPos = startPos;
        finalPos.X += initialVelocity.X * flightDuration;
        finalPos.Y += initialVelocity.Y * flightDuration;
        return finalPos;
    }

    public static Vector3? CalculateFireVector(Vector3 origin, Vector3 targetPos)
    {
        /*
        g: Acceleration due to gravity.
        d: Horizontal distance the projectile must travel.
        v: Initial velocity of the projectile.
        y0: Initial height from which the projectile is launched.
        y: Final height where the projectile lands.
        */

        Vector3 delta = targetPos - origin;
        Vector2 xyDelta = new Vector2(delta.X, delta.Y);
        float g = Constants.ArrowGravity;
        float d = xyDelta.Length();
        float v = d > 2 ? Constants.ArrowVelocity : Constants.ArrowWeakVelocity;
        float y0 = origin.Z;
        float y = targetPos.Z;

        float quadA = (-1 * g * d * d) / (2 * v * v);
        float quadB = d / v;
        float quadC = (g * d * d) / (2 * v * v) + (y0 - y);

        float discriminant = quadB * quadB - 4 * quadA * quadC;
        if (discriminant < 0)
        {
            return null;
        }

        float solutionA = (-quadB + MathF.Sqrt(discriminant)) / (2 * quadA);
        float solutionB = (-quadB - MathF.Sqrt(discriminant)) / (2 * quadA);
        float tanTheta = MathF.Max(solutionA, solutionB);
        float theta = MathF.Atan(tanTheta);

        xyDelta /= xyDelta.Length();
        float nonVerticalVelocity = MathF.Cos(theta) * v;
        return new Vector3(
            x: xyDelta.X * nonVerticalVelocity,
            y: xyDelta.Y * nonVerticalVelocity,
            z: MathF.Sin(theta) * v
        );
    }
}