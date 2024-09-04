using Schema;

namespace KeepLordWarriors;

public class Player
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public List<Oneof_GameServerToPlayer> MessageQueue { get; } = [];

    public Player(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public void EnqueuePackets(List<Oneof_GameServerToPlayer> messages)
    {
        foreach (Oneof_GameServerToPlayer msg in messages)
        {
            MessageQueue.Add(msg);
        }
    }
}