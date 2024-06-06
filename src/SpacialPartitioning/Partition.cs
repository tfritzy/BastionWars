namespace BastionWars;

public class Partition
{
    List<Entity> entities;

    public Partition()
    {
        entities = new();
    }

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }
}