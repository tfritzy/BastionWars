using System.Numerics;
using System.Runtime.CompilerServices;
using Schema;

namespace KeepLordWarriors;

public class Keep : Entity
{
    public int ArcherCount { get; private set; }
    public int WarriorCount { get; private set; }
    public int MageCount { get; private set; }
    public SoldierType SoldierType { get; }
    public List<DeploymentOrder> DeploymentOrders { get; } = [];
    public string? Name { get; set; }
    public bool SomethingChanged { get; private set; }

    public const float Radius = 2f;
    public const float DeploymentRefractoryPeriod = .5f;
    public const int MaxTroopsPerWave = 3;
    public const float RowMaxWidth = .4f;
    public const float RowOffsetStepSize = RowMaxWidth / MaxTroopsPerWave;
    public const float RowOffsetLeft = -RowMaxWidth / 2;
    public const int StartTroopCount = 5;
    public const float TargetCheckTime = .2f;

    // Overkill damage during a breach is stored here and applied to the next wave
    private float powerOverflow;
    private float targetCheckCooldown = TargetCheckTime;
    private List<float> archerFireCooldowns = [];
    private List<uint> archerTargets = [];

    public Keep(Game game, SoldierType soldierType, int alliance = 0) : base(game, alliance)
    {
        SoldierType = soldierType;
        SetCount(soldierType, StartTroopCount);
        SomethingChanged = false;
    }

    public void Update()
    {
        FireUponIntruders();
        DeployTroops();
    }

    private void FireUponIntruders()
    {
        if (ArcherCount > 0 || MageCount > 0)
        {
            targetCheckCooldown -= Game.Time.deltaTime;
            if (targetCheckCooldown <= 0)
            {
                targetCheckCooldown = TargetCheckTime;
                List<uint> inRange = Game.Map.Grid.GetCollisions(
                    Game.Map.Grid.GetEntityPosition(Id),
                    Constants.ArcherBaseRange);
                for (int i = inRange.Count - 1; i >= 0; i--)
                {
                    if (!Game.Map.Soldiers.ContainsKey(inRange[i]))
                    {
                        inRange.RemoveAt(i);
                    }
                    else if (Game.Map.Soldiers[inRange[i]].Alliance == Alliance)
                    {
                        inRange.RemoveAt(i);
                    }
                }
                archerTargets = inRange;
            }
        }

        if (archerTargets.Count > 0)
        {
            while (archerFireCooldowns.Count < ArcherCount)
            {
                float setupTime = Randy.ChaoticInRange(Constants.ArcherSetupMinTime, Constants.ArcherSetupMaxTime);
                archerFireCooldowns.Add(setupTime);
            }

            while (archerFireCooldowns.Count > ArcherCount)
            {
                archerFireCooldowns.RemoveAt(archerFireCooldowns.Count - 1);
            }

            for (int i = 0; i < archerFireCooldowns.Count; i++)
            {
                archerFireCooldowns[i] -= Game.Time.deltaTime;
                if (archerFireCooldowns[i] <= 0)
                {
                    FireArrow();
                    archerFireCooldowns[i] = Constants.ArcherBaseCooldown;
                }
            }
        }
    }

    private void FireArrow()
    {
        if (archerTargets.Count == 0)
            return;

        uint target = Randy.ChaoticElement(archerTargets);

        if (!Game.Map.Grid.ContainsEntity(target))
            return;

        Soldier s = Game.Map.Soldiers[target];

        Vector2 startPos2D = Game.Map.Grid.GetEntityPosition(Id);
        startPos2D.X += Randy.ChaoticInRange(-.2f, .2f) + .5f;
        startPos2D.Y += Randy.ChaoticInRange(-.2f, .2f) + .5f;
        Vector2 targetPos2D = Game.Map.Grid.GetEntityPosition(target);
        Vector3 startPos = new Vector3(startPos2D.X, startPos2D.Y, 0);
        Vector3 targetPos = new Vector3(targetPos2D.X, targetPos2D.Y, 0);
        targetPos.X += Randy.ChaoticInRange(-.1f, .1f);
        targetPos.Y += Randy.ChaoticInRange(-.1f, .1f);
        float distance = (targetPos2D - startPos2D).Length();
        float roughFlightTime = distance / (Constants.ArrowVelocity * .8f);
        targetPos.X += s.Velocity.X * roughFlightTime;
        targetPos.Y += s.Velocity.Y * roughFlightTime;
        Vector3? velocity = Projectile.CalculateFireVector(startPos, targetPos);

        if (velocity == null)
        {
            Logger.Log($"ERROR - could not calculate a vector to shoot {targetPos} from {startPos}. Skipping arrow");
            return;
        }

        Projectile projectile = new Projectile(
            startPos: startPos,
            birthTime: Game.Time.Now,
            initialVelocity: velocity.Value,
            Alliance,
            Constants.ArrowBaseDamage
        );
        Game.Map.AddProjectile(projectile);
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
        if (archers != null) SetCount(SoldierType.Archer, archers.Value);
        if (warriors != null) SetCount(SoldierType.Warrior, warriors.Value);
    }

    public void SetCount(SoldierType type, int count)
    {
        switch (type)
        {
            case SoldierType.Archer:
                ArcherCount = count;
                break;
            case SoldierType.Warrior:
                WarriorCount = count;
                break;
        }

        SomethingChanged = true;
    }

    public void Accrue()
    {
        IncrementSoldierCount(SoldierType, 1);
    }

    public void IncrementSoldierCount(SoldierType type, int amount = 1)
    {
        SetCount(type, GetCount(type) + amount);
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
            if (order.WaveCooldown > 0)
            {
                order.WaveCooldown -= Game.Time.deltaTime;
            }
        }

        for (int i = 0; i < DeploymentOrders.Count; i++)
        {
            var order = DeploymentOrders[i];
            if (order.WaveCooldown <= 0)
            {
                order.WaveCooldown = DeploymentRefractoryPeriod;
                int waveCap = MaxTroopsPerWave;

                int toDeploy = Min(WarriorCount, waveCap, order.WarriorCount);
                float rowOffset = RowOffsetLeft;
                SetCount(SoldierType.Warrior, WarriorCount - toDeploy);
                order.WarriorCount -= toDeploy;
                waveCap -= toDeploy;

                for (int j = 0; j < toDeploy; j++)
                {
                    Game.Map.AddSoldier(
                        new Soldier(Game, Alliance, SoldierType.Warrior, Id, order.TargetId, rowOffset),
                        Game.Map.Grid.GetEntityPosition(Id));
                    rowOffset += RowOffsetStepSize;
                }

                toDeploy = Min(ArcherCount, waveCap, order.ArcherCount);
                SetCount(SoldierType.Archer, ArcherCount - toDeploy);
                order.ArcherCount -= toDeploy;

                for (int j = 0; j < toDeploy; j++)
                {
                    Game.Map.AddSoldier(
                        new Soldier(Game, Alliance, SoldierType.Archer, Id, order.TargetId, rowOffset),
                        Game.Map.Grid.GetEntityPosition(Id));
                    rowOffset += RowOffsetStepSize;
                }

                if (order.ArcherCount == 0 && order.WarriorCount == 0)
                {
                    Logger.Log($"Removing deployment order from keep {Id} at index {i} when count is {DeploymentOrders.Count}");
                    DeploymentOrders.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void SetDeploymentOrder(uint target, SoldierType? type = null, float percent = 1f)
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
            float attackerPower = Game.Map.MeleePowerOf(soldier);
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
                    powerOverflow = attackerPower - Game.Map.MeleePowerOf(soldier);
                    break;
                }
            }
        }
    }

    private float DoBattle(float attackerPower, SoldierType defenderType)
    {
        float defenderPower = Game.Map.MeleePowerOf(defenderType, Alliance);
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

    public void Capture(int alliance)
    {
        Alliance = alliance;
        SomethingChanged = true;
        Game.Map.RecalculateRenderTiles();
    }

    public void AckOccupancyChanged()
    {
        SomethingChanged = false;
    }
}