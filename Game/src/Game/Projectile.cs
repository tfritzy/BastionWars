using System;
using System.Numerics;

namespace KeepLordWarriors;

public class Projectile(Vector3 startPos, float birthTime, Vector3 initialVelocity)
{
    public uint Id { get; private set; } = IdGenerator.NextId();
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
        float s = Constants.ArrowVelocity; // d > 2 ? Constants.ArrowVelocity : Constants.ArrowWeakVelocity;
        float y0 = origin.Z;
        float y = targetPos.Z;

        var constants = (d * g) / (s * s);
        var theta = MathF.Asin(constants) / 2;

        // theta = (MathF.PI / 2) - theta; // Convert to high arc

        Logger.Log("Firing projectile at angle: " + theta * 180f / MathF.PI);

        if (float.IsNaN(theta))
        {
            return null;
        }

        float verticalVelocity = MathF.Sin(theta) * s;
        float nonVerticalVelocity = MathF.Cos(theta) * s;

        xyDelta /= xyDelta.Length();

        return new Vector3(
            x: xyDelta.X * nonVerticalVelocity,
            y: xyDelta.Y * nonVerticalVelocity,
            z: verticalVelocity
        );
    }
}