namespace Tests;

public static class TH
{
    public static Entity BuildEntity(float x, float y, float radius = .2f)
    {
        return new Entity(x, y, 1, radius);
    }
}