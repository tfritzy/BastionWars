using System.Numerics;

namespace KeepLordWarriors;

public class Projectile(Vector3 startPos, float birthTime, Vector3 initialVelocity)
{
    public Vector3 StartPos { get; private set; } = startPos;
    public float BirthTime { get; private set; } = birthTime;
    public Vector3 InitialVelocity { get; private set; } = initialVelocity;
    public float TimeWillLand { get; private set; } = birthTime + GetFlightDuration(initialVelocity);
    public Vector3 LandPosition { get; private set; } = GetLandPosition(startPos, initialVelocity);

    private static float GetFlightDuration(Vector3 initialVelocity)
    {
        float timeToPeak = Math.Abs(initialVelocity.Z) / 9.81f;
        return timeToPeak * 2;
    }

    private static Vector3 GetLandPosition(Vector3 startPos, Vector3 initialVelocity)
    {
        float flightDuration = GetFlightDuration(initialVelocity);
        Vector3 finalPos = startPos;
        finalPos.X += initialVelocity.X * flightDuration;
        finalPos.Y += initialVelocity.Y * flightDuration;
        return finalPos;
    }
}