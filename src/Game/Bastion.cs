namespace KeepLordWarriors;

public class Bastion : Entity
{
    public int MageCount { get; private set; }
    public int ArcherCount { get; private set; }
    public int WarriorCount { get; private set; }
    public SoldierType SoldierType { get; }

    public DeploymentOrder DeploymentOrder { get; private set; }
    private float deploymentCooldown = 0f;

    public const float Radius = 2f;
    public const float DeploymentRefractoryPeriod = .75f;

    public Bastion(SoldierType soldierType, int alliance = 0) : base(alliance)
    {
        SoldierType = soldierType;
    }

    public int GetCount(SoldierType type)
    {
        return type switch
        {
            SoldierType.Mage => MageCount,
            SoldierType.Archer => ArcherCount,
            SoldierType.Warrior => WarriorCount,
            _ => 0
        };
    }

    public void SetCount(int? mages = null, int? archers = null, int? warriors = null)
    {
        MageCount = mages ?? MageCount;
        ArcherCount = archers ?? ArcherCount;
        WarriorCount = warriors ?? WarriorCount;
    }

    public void Produce()
    {
        switch (SoldierType)
        {
            case SoldierType.Mage:
                MageCount++;
                break;
            case SoldierType.Archer:
                ArcherCount++;
                break;
            case SoldierType.Warrior:
                WarriorCount++;
                break;
        }
    }

    public void Update(float deltaTime)
    {
        if (deploymentCooldown > 0)
        {
            deploymentCooldown -= deltaTime;
        }

        DeployTroops();
    }

    private void DeployTroops()
    {
        if (deploymentCooldown <= 0)
        {
            deploymentCooldown = DeploymentRefractoryPeriod;
        }
    }

    public void SetDeploymentOrder(DeploymentOrder order)
    {
        DeploymentOrder = order;
    }
}