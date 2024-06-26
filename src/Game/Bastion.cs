using SpacialPartitioning;

namespace KeepLordWarriors;

public class Bastion : Entity
{
    public int ArcherCount { get; private set; }
    public int WarriorCount { get; private set; }
    public SoldierType SoldierType { get; }
    public Map Map { get; private set; }

    public List<DeploymentOrder> DeploymentOrders { get; } = new();

    public const float Radius = 2f;
    public const float DeploymentRefractoryPeriod = .75f;
    public const int MaxTroopsPerWave = 6;

    /// Overkill damage during a breach is stored here and applied to the next wave
    private float powerOverflow;

    public Bastion(Map map, SoldierType soldierType, int alliance = 0) : base(map, alliance)
    {
        SoldierType = soldierType;
        Map = map;
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

    public void Accrue()
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

    public void Update(double deltaTime)
    {
        foreach (var order in DeploymentOrders)
        {
            if (order.WaveCooldown > 0)
            {
                order.WaveCooldown -= deltaTime;
            }
        }

        DeployTroops();
    }

    private void DeployTroops()
    {
        if (DeploymentOrders.Count == 0)
        {
            return;
        }

        for (int i = 0; i < DeploymentOrders.Count; i++)
        {
            var order = DeploymentOrders[i];
            if (order.WaveCooldown <= 0)
            {
                order.WaveCooldown = DeploymentRefractoryPeriod;
                int waveCap = MaxTroopsPerWave;

                int toDeploy = Min(WarriorCount, waveCap, order.WarriorCount);
                WarriorCount -= toDeploy;
                order.WarriorCount -= toDeploy;
                waveCap -= toDeploy;

                for (int j = 0; j < toDeploy; j++)
                {
                    Map.AddSoldier(
                        new Soldier(Map, Alliance, SoldierType.Warrior, Id, order.TargetId),
                        Map.Grid.GetEntityPosition(Id));
                }

                toDeploy = Min(ArcherCount, waveCap, order.ArcherCount);
                ArcherCount -= toDeploy;
                order.ArcherCount -= toDeploy;

                for (int j = 0; j < toDeploy; j++)
                {
                    Map.AddSoldier(
                        new Soldier(Map, Alliance, SoldierType.Archer, Id, order.TargetId),
                        Map.Grid.GetEntityPosition(Id));
                }

                if (order.ArcherCount == 0 && order.WarriorCount == 0)
                {
                    DeploymentOrders.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void SetDeploymentOrder(ulong target, SoldierType? type = null, float percent = 1f)
    {
        var deploymentOrder = new DeploymentOrder
        {
            TargetId = target,
            ArcherCount = type == SoldierType.Archer || type == null ? (int)(ArcherCount * percent) : 0,
            WarriorCount = type == SoldierType.Warrior || type == null ? (int)(WarriorCount * percent) : 0,
        };

        int existingIndex = DeploymentOrders.FindIndex(o => o.TargetId == target);
        if (existingIndex >= 0)
        {
            deploymentOrder.WaveCooldown = DeploymentOrders[existingIndex].WaveCooldown;
            DeploymentOrders[existingIndex] = deploymentOrder;
        }
        else
        {
            DeploymentOrders.Add(deploymentOrder);
        }

        DeployTroops();
    }

    private static int Min(params int[] values)
    {
        return values.Min();
    }

    public void Breach(Soldier soldier)
    {
        if (soldier.Alliance == Alliance)
        {
            IncrementSoldierCount(soldier.Type);
        }
        else
        {
            float attackerPower = map.MeleePowerOf(soldier);
            while (attackerPower > 0)
            {
                if (WarriorCount > 0)
                {
                    attackerPower = DoBattle(attackerPower, SoldierType.Warrior);
                }
                else if (ArcherCount > 0)
                {
                    attackerPower = DoBattle(attackerPower, SoldierType.Archer);
                }
                else
                {
                    Capture(soldier.Alliance);
                    IncrementSoldierCount(soldier.Type);
                    powerOverflow = attackerPower - map.MeleePowerOf(soldier);
                    break;
                }
            }
        }
    }

    private float DoBattle(float attackerPower, SoldierType defenderType)
    {
        float defenderPower = map.MeleePowerOf(defenderType, Alliance);
        if (attackerPower >= defenderPower + powerOverflow)
        {
            // attacker wins
            float remainingPower = attackerPower - defenderPower - powerOverflow;
            IncrementSoldierCount(defenderType, -1);
            powerOverflow = 0;
            return remainingPower;
        }
        else
        {
            // defender wins
            powerOverflow -= attackerPower;
            return 0;
        }
    }

    private void IncrementSoldierCount(SoldierType type, int amount = 1)
    {
        switch (type)
        {
            case SoldierType.Archer:
                ArcherCount += amount;
                break;
            case SoldierType.Warrior:
                WarriorCount += amount;
                break;
        }
    }

    public void Capture(int alliance)
    {
        Alliance = alliance;
    }
}