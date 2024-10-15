using Schema;

namespace KeepLordWarriors;

public class Player
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public List<Oneof_GameServerToPlayer> MessageQueue { get; } = [];
    public int Alliance { get; set; }

    public Player(string name, string id)
    {
        Name = name;
        Id = id;
    }
}