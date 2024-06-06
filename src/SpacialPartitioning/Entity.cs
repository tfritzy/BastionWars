namespace BastionWars;

public class Entity
{
    public float x;
    public float y;
    public ulong id;
    public float radius;

    public Entity(float x, float y, ulong id, float radius)
    {
        this.x = x;
        this.y = y;
        this.id = id;
        this.radius = radius;
    }
}
