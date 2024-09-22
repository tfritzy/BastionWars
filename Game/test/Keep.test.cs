using System.Numerics;
using KeepLordWarriors;
using TestHelpers;

namespace Tests;

[TestClass]
public class KeepTest
{
    [TestMethod]
    public void Keep_FiresAtEnemiesInRange()
    {
        Game game = new(TH.GetGameSettings());
        Keep keep = game.Map.KeepAt(0);
        game.Map.KeepAt(0).SetCount(archers: 10);

        game.Update(5f);

        Assert.AreEqual(0, game.Map.Projectiles.Count);
        game.Map.AddSoldier(
            TH.BuildEnemySoldier(Schema.SoldierType.Warrior, keep.Alliance, game.Map),
            game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));

        game.Update(1);
        Assert.AreEqual(10, game.Map.Projectiles.Count);
    }
}
