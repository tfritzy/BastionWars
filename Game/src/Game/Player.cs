using Schema;

namespace KeepLordWarriors;

public class Player
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public List<OneofUpdate> HoldingArea { get; } = new();
    public List<Packet> PendingPackets { get; } = new();
    public ulong HighestPacketId { get; private set; } = 0;

    public Player(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public void EnqueuePackets(List<Packet> packets)
    {
        foreach (Packet packet in packets)
        {
            PendingPackets.Add(packet);
        }

        HighestPacketId = packets.Last().Id;
    }
}