namespace KeepLordWarriors;

public class Soldier : Entity
{
    private ulong targetBastionId;
    private ulong sourceBastionId;
    private int pathProgress;

    public Soldier(int alliance) : base(alliance)
    {
    }
}