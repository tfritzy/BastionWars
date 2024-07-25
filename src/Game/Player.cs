namespace KeepLordWarriors;

public class Player
{
    public string Name { get; private set; }
    public string Id { get; private set; }

    public Player(string name, string id)
    {
        Name = name;
        Id = id;
    }
}