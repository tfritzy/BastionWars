using Tests;

namespace KeepLordWarriors;

[TestClass]
public class GameTests
{
    [TestMethod]
    public void Game_AutoAccrues()
    {
        Game game = new(new GameSettings(GenerationMode.AutoAccrue, TestMaps.TenByFive));

        game.Update(Game.AutoAccrualTime - .1f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(0, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(.2f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(1, bastion.GetCount(bastion.SoldierType));
        }
        game.Update(Game.AutoAccrualTime - .1f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(1, bastion.GetCount(bastion.SoldierType));
        }
    }

    [TestMethod]
    public void Game_DoesntAccrueIfWordMode()
    {
        Game game = new(new GameSettings(GenerationMode.Word, TestMaps.TenByFive));

        game.Update(Game.AutoAccrualTime + .1f);
        foreach (Bastion bastion in game.Map.Bastions)
        {
            Assert.AreEqual(0, bastion.GetCount(bastion.SoldierType));
        }
    }
}