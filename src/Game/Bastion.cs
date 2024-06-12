namespace KeepLordWarriors;

public class Bastion : Entity
{
    public int ArcherCount { get; private set; }
    public int WarriorCount { get; private set; }
    public SoldierType SoldierType { get; }

    public DeploymentOrder? DeploymentOrder { get; private set; }
    private float deploymentCooldown = 0f;

    public const float Radius = 2f;
    public const float DeploymentRefractoryPeriod = .75f;
    public const int MaxTroopsPerWave = 6;

    public Bastion(SoldierType soldierType, int alliance = 0) : base(alliance)
    {
        SoldierType = soldierType;
    }

    public int GetCount(SoldierType type)
    {
        return type switch
        {
            SoldierType.Archer => ArcherCount,
            SoldierType.Warrior => WarriorCount,
            _ => 0
        };
    }

    public void SetCount(int? archers = null, int? warriors = null)
    {
        ArcherCount = archers ?? ArcherCount;
        WarriorCount = warriors ?? WarriorCount;
    }

    public void Produce()
    {
        switch (SoldierType)
        {
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
        if (DeploymentOrder == null)
        {
            return;
        }

        if (deploymentCooldown <= 0)
        {
            deploymentCooldown = DeploymentRefractoryPeriod;
            int waveCap = MaxTroopsPerWave;

            int toDeploy = Min(WarriorCount, waveCap, DeploymentOrder.WarriorCount);
            WarriorCount -= toDeploy;
            DeploymentOrder.WarriorCount -= toDeploy;
            waveCap -= toDeploy;

            toDeploy = Min(ArcherCount, waveCap, DeploymentOrder.ArcherCount);
            ArcherCount -= toDeploy;
            DeploymentOrder.ArcherCount -= toDeploy;

            if (DeploymentOrder.ArcherCount == 0 && DeploymentOrder.WarriorCount == 0)
            {
                DeploymentOrder = null;
            }
        }
    }

    public void SetDeploymentOrder(ulong target, SoldierType? type = null, float percent = 1f)
    {
        DeploymentOrder = new DeploymentOrder
        {
            TargetId = target,
            ArcherCount = type == SoldierType.Archer || type == null ? (int)(ArcherCount * percent) : 0,
            WarriorCount = type == SoldierType.Warrior || type == null ? (int)(WarriorCount * percent) : 0,
        };

        DeployTroops();
    }

    private static int Min(params int[] values)
    {
        return values.Min();
    }
}