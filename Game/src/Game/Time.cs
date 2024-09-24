namespace KeepLordWarriors;

public class Time
{
    public float Now = 0f;
    public float deltaTime;

    public void Update(float now)
    {
        deltaTime = now - Now;
        Now = now;
    }
}