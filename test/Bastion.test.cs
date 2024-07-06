using KeepLordWarriors;

namespace Tests;

[TestClass]
public class BastionTests
{
    [TestMethod]
    public void Bastion_BasicStuff()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);
        Assert.AreEqual(0, bastion.ArcherCount);
        Assert.AreEqual(0, bastion.WarriorCount);
        Assert.AreEqual(SoldierType.Warrior, bastion.SoldierType);
        Assert.AreNotEqual(0u, bastion.Id);
        Keep bastion2 = new(map, SoldierType.Archer, 3);
        Assert.AreEqual(bastion.Id + 1, bastion2.Id);
        Assert.AreEqual(3, bastion2.Alliance);
    }

    [TestMethod]
    public void Bastion_IncreasesPopulationAppropriately()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        foreach (var type in Enum.GetValues<SoldierType>())
        {
            Keep bastion = new(map, type);
            Assert.AreEqual(0, bastion.GetCount(type));
            bastion.Accrue();
            Assert.AreEqual(1, bastion.GetCount(type));
        }
    }


    [TestMethod]
    public void Map_BastionAttackDeploysCorrectTroops()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = map.Keeps[0];

        bastion.SetCount(archers: 30);
        bastion.SetDeploymentOrder(2u);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Archer));

        bastion.SetCount(archers: 30);
        bastion.SetDeploymentOrder(2u, type: SoldierType.Warrior);
        bastion.Update(Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(30, bastion.GetCount(SoldierType.Archer));

        bastion.SetCount(warriors: 30);
        bastion.SetDeploymentOrder(2u, type: SoldierType.Warrior);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Warrior));

        bastion.SetCount(warriors: 30, archers: 30);
        bastion.SetDeploymentOrder(2u, percent: 1f);
        bastion.Update(Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30, bastion.GetCount(SoldierType.Archer));

        bastion.SetCount(warriors: 4, archers: 10);
        bastion.SetDeploymentOrder(2u, percent: 1f);
        bastion.Update(Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(0, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(8, bastion.GetCount(SoldierType.Archer));

        bastion.SetCount(warriors: 4, archers: 10);
        bastion.SetDeploymentOrder(2u, percent: .5f);
        bastion.Update(Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(2, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(6, bastion.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Bastion_DeploysTroopsOverTime()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = map.Keeps[1];
        bastion.SetCount(warriors: 30);
        bastion.SetDeploymentOrder(0, SoldierType.Warrior, .5f);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Warrior));
        bastion.Update(Keep.DeploymentRefractoryPeriod - .1f);
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Warrior));
        bastion.Update(.11f);
        Assert.AreEqual(18, bastion.GetCount(SoldierType.Warrior));
        bastion.Update(10f);
        Assert.AreEqual(15, bastion.GetCount(SoldierType.Warrior));
    }

    [TestMethod]
    public void Bastion_MultipleOrders()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = map.Keeps[2];
        bastion.SetCount(warriors: 15, archers: 30);

        bastion.SetDeploymentOrder(map.Keeps[0].Id, SoldierType.Warrior, 1f);
        Assert.AreEqual(9, bastion.GetCount(SoldierType.Warrior));

        bastion.Update(Keep.DeploymentRefractoryPeriod / 2);

        bastion.SetDeploymentOrder(map.Keeps[1].Id, SoldierType.Archer, 1f);
        Assert.AreEqual(9, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Archer));

        bastion.Update(Keep.DeploymentRefractoryPeriod / 2 + .1f);
        Assert.AreEqual(3, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(24, bastion.GetCount(SoldierType.Archer));

        bastion.Update(Keep.DeploymentRefractoryPeriod / 2 + .1f);
        Assert.AreEqual(3, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(18, bastion.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Bastion_Breach_GetsCaptured()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);

        bastion.SetCount(warriors: 30);
        int alliance = bastion.Alliance;
        for (int i = 0; i < 31; i++)
        {
            bastion.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, map));
        }
        Assert.AreEqual(1, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(0, bastion.GetCount(SoldierType.Archer));
        Assert.AreEqual(alliance + 1, bastion.Alliance); // captured
    }

    [TestMethod]
    public void Bastion_Breach_FightsSoldiersFirst()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);

        bastion.SetCount(warriors: 10, archers: 30);
        int alliance = bastion.Alliance;
        for (int i = 0; i < 6; i++)
        {
            bastion.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, map));
        }
        Assert.AreEqual(4, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30, bastion.GetCount(SoldierType.Archer));

        for (int i = 0; i < 6; i++)
        {
            bastion.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, map));
        }
        Assert.AreEqual(0, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(22, bastion.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Bastion_Breach_SoldiersWreckArchers()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);

        bastion.SetCount(archers: 30);
        int alliance = bastion.Alliance;
        for (int i = 0; i < 5; i++)
        {
            bastion.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, map));
        }
        Assert.AreEqual(10, bastion.GetCount(SoldierType.Archer));
        Assert.AreEqual(alliance, bastion.Alliance); // not captured
    }

    [TestMethod]
    public void Bastion_Breach_ArchersNotVeryGoodAtIt()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);

        bastion.SetCount(warriors: 30);
        int alliance = bastion.Alliance;
        for (int i = 0; i < 90; i++)
        {
            bastion.Breach(TH.BuildEnemySoldier(SoldierType.Archer, alliance, map));
        }
        Assert.AreEqual(8, bastion.GetCount(SoldierType.Warrior));
        Assert.AreEqual(alliance, bastion.Alliance); // not captured
    }

    [TestMethod]
    public void Bastion_Breach_PowerOverflowOnCapture()
    {
        KeepLordWarriors.Map map = new(TestMaps.TenByFive);
        Keep bastion = new(map, SoldierType.Warrior);
        int originalAlliance = bastion.Alliance;

        bastion.SetCount(archers: 2);
        bastion.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, originalAlliance, map));
        Assert.AreEqual(originalAlliance + 1, bastion.Alliance); // captured
        Assert.AreEqual(1, bastion.GetCount(SoldierType.Warrior)); // should have 2 power left
        for (int i = 0; i < 3; i++)
        {
            bastion.Breach(TH.BuildAllySoldier(SoldierType.Archer, originalAlliance, map));
        }
        Assert.AreEqual(originalAlliance, bastion.Alliance); // re-captured
        Assert.AreEqual(1, bastion.GetCount(SoldierType.Archer));
    }
}