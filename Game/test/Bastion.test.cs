using KeepLordWarriors;
using Schema;
using TestHelpers;

namespace Tests;

[TestClass]
public class KeepTests
{
    [TestMethod]
    public void Keep_BasicStuff()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);
        Assert.AreEqual(0, keep.ArcherCount);
        Assert.AreEqual(5, keep.WarriorCount);
        Assert.AreEqual(SoldierType.Warrior, keep.SoldierType);
        Assert.AreNotEqual(0u, keep.Id);
        Keep keep2 = new(game, SoldierType.Archer, 3);
        Assert.AreEqual(keep.Id + 1, keep2.Id);
        Assert.AreEqual(3, keep2.Alliance);
    }

    [TestMethod]
    public void Keep_IncreasesPopulationAppropriately()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        foreach (var type in Enum.GetValues<SoldierType>())
        {
            if (type == SoldierType.InvalidSoldier)
                continue;

            Keep keep = new(game, type);
            Assert.AreEqual(Keep.StartTroopCount, keep.GetCount(type));
            keep.Accrue();
            Assert.AreEqual(Keep.StartTroopCount + 1, keep.GetCount(type));
        }
    }


    [TestMethod]
    public void Map_KeepAttackDeploysCorrectTroops()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = game.Map.KeepAt(0);
        Keep target = game.Map.KeepAt(5);

        keep.SetCount(warriors: 0, archers: 30);
        keep.SetDeploymentOrder(target.Id);
        TH.UpdateGame(game, .0001f);
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Archer));

        keep.SetCount(archers: 30);
        keep.SetDeploymentOrder(target.Id, type: SoldierType.Warrior);
        TH.UpdateGame(game, .0001f);
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(30, keep.GetCount(SoldierType.Archer));

        keep.SetCount(warriors: 30);
        keep.SetDeploymentOrder(target.Id, type: SoldierType.Warrior);
        TH.UpdateGame(game, .0001f);
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Warrior));

        keep.SetCount(warriors: 30, archers: 30);
        keep.SetDeploymentOrder(target.Id, percent: 1f);
        TH.UpdateGame(game, .0001f);
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30, keep.GetCount(SoldierType.Archer));

        keep.SetCount(warriors: 4, archers: 10);
        keep.SetDeploymentOrder(target.Id, percent: 1f);
        TH.UpdateGame(game, .0001f);
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(0, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(8, keep.GetCount(SoldierType.Archer));

        keep.SetCount(warriors: 4, archers: 10);
        keep.SetDeploymentOrder(target.Id, percent: .5f);
        TH.UpdateGame(game, .0001f);
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod);
        Assert.AreEqual(2, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(6, keep.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Keep_DeploysTroopsOverTime()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = game.Map.KeepAt(1);
        Keep target = game.Map.KeepAt(5);
        keep.SetCount(warriors: 30, archers: 0);
        keep.SetDeploymentOrder(target.Id, SoldierType.Warrior, .5f);
        TH.UpdateGame(game, .0001f);
        Assert.AreEqual(24, keep.GetCount(SoldierType.Warrior));
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod - .1f);
        Assert.AreEqual(24, keep.GetCount(SoldierType.Warrior));
        TH.UpdateGame(game, .11f);
        Assert.AreEqual(18, keep.GetCount(SoldierType.Warrior));
        TH.UpdateGame(game, 10f);
        Assert.AreEqual(15, keep.GetCount(SoldierType.Warrior));
    }

    [TestMethod]
    public void Keep_MultipleOrders()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = game.Map.KeepAt(2);
        keep.SetCount(warriors: 15, archers: 30);

        keep.SetDeploymentOrder(game.Map.KeepAt(0).Id, SoldierType.Warrior, 1f);
        TH.UpdateGame(game, .0001f);
        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod / 2);

        keep.SetDeploymentOrder(game.Map.KeepAt(1).Id, SoldierType.Archer, 1f);
        TH.UpdateGame(game, .0001f);
        Assert.AreEqual(15 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Archer));

        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod / 2 + .1f);
        Assert.AreEqual(15 - Keep.MaxTroopsPerWave * 2, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave, keep.GetCount(SoldierType.Archer));

        TH.UpdateGame(game, Keep.DeploymentRefractoryPeriod / 2 + .1f);
        Assert.AreEqual(15 - Keep.MaxTroopsPerWave * 2, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30 - Keep.MaxTroopsPerWave * 2, keep.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Keep_Breach_GetsCaptured()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);

        keep.SetCount(warriors: 30);
        int alliance = keep.Alliance;
        for (int i = 0; i < 31; i++)
        {
            keep.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, game));
        }
        Assert.AreEqual(1, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(0, keep.GetCount(SoldierType.Archer));
        Assert.AreEqual(alliance + 1, keep.Alliance); // captured
    }

    [TestMethod]
    public void Keep_Breach_FightsSoldiersFirst()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);

        keep.SetCount(warriors: 10, archers: 30);
        int alliance = keep.Alliance;
        for (int i = 0; i < 6; i++)
        {
            keep.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, game));
        }
        Assert.AreEqual(4, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(30, keep.GetCount(SoldierType.Archer));

        for (int i = 0; i < 6; i++)
        {
            keep.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, game));
        }
        Assert.AreEqual(0, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(22, keep.GetCount(SoldierType.Archer));
    }

    [TestMethod]
    public void Keep_Breach_SoldiersWreckArchers()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);

        keep.SetCount(archers: 30, warriors: 0);
        int alliance = keep.Alliance;
        for (int i = 0; i < 5; i++)
        {
            keep.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, alliance, game));
        }
        Assert.AreEqual(10, keep.GetCount(SoldierType.Archer));
        Assert.AreEqual(alliance, keep.Alliance); // not captured
    }

    [TestMethod]
    public void Keep_Breach_ArchersNotVeryGoodAtIt()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);

        keep.SetCount(warriors: 30);
        int alliance = keep.Alliance;
        for (int i = 0; i < 90; i++)
        {
            keep.Breach(TH.BuildEnemySoldier(SoldierType.Archer, alliance, game));
        }
        Assert.AreEqual(8, keep.GetCount(SoldierType.Warrior));
        Assert.AreEqual(alliance, keep.Alliance); // not captured
    }

    [TestMethod]
    public void Keep_Breach_PowerOverflowOnCapture()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        Keep keep = new(game, SoldierType.Warrior);
        int originalAlliance = keep.Alliance;

        keep.SetCount(archers: 2, warriors: 0);
        keep.Breach(TH.BuildEnemySoldier(SoldierType.Warrior, originalAlliance, game));
        Assert.AreEqual(originalAlliance + 1, keep.Alliance); // captured
        Assert.AreEqual(1, keep.GetCount(SoldierType.Warrior)); // should have 2 power left
        for (int i = 0; i < 3; i++)
        {
            keep.Breach(TH.BuildAllySoldier(SoldierType.Archer, originalAlliance, game));
        }
        Assert.AreEqual(originalAlliance, keep.Alliance); // re-captured
        Assert.AreEqual(1, keep.GetCount(SoldierType.Archer));
    }
}