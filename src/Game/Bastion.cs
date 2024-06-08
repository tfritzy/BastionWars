namespace BastionWars;

public class Bastion : Entity
{
    public int MageCount { get; private set; }
    public int ArcherCount { get; private set; }
    public int WarriorCount { get; private set; }
    public BastionType Type { get; }

    public const float Radius = 2f;

    public Bastion(BastionType type, int alliance = 0) : base(alliance)
    {
        Type = type;
    }

    public int GetCount(BastionType type)
    {
        return type switch
        {
            BastionType.Mage => MageCount,
            BastionType.Archer => ArcherCount,
            BastionType.Warrior => WarriorCount,
            _ => 0
        };
    }

    public void Produce()
    {
        switch (Type)
        {
            case BastionType.Mage:
                MageCount++;
                break;
            case BastionType.Archer:
                ArcherCount++;
                break;
            case BastionType.Warrior:
                WarriorCount++;
                break;
        }
    }
}