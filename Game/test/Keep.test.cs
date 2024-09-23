using System.Numerics;
using KeepLordWarriors;
using Schema;
using TestHelpers;

namespace Tests;

[TestClass]
public class KeepTest
{
    [TestMethod]
    public void Keep_FiresAtEnemiesInRange()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        Keep keep = game.Map.KeepAt(0);
        foreach (Keep k in game.Map.Keeps.Values)
        {
            // Ensure only keep 0 will be the one firing.
            if (k != keep)
                k.Alliance = 2;
        }
        game.Map.KeepAt(0).SetCount(archers: 10);

        TH.UpdateGame(game, 5f);

        Assert.AreEqual(0, game.Map.Projectiles.Count);
        game.Map.AddSoldier(
            TH.BuildEnemySoldier(Schema.SoldierType.Warrior, keep.Alliance, game.Map),
            game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));

        TH.UpdateGame(game, 1);
        Assert.AreEqual(10, game.Map.Projectiles.Count);
    }

    [TestMethod]
    public void Keep_CanShootEverywhereInItsRange()
    {
        Vector3 origin = new Vector3(0, 0, 10);
        AssertIsSensibleShot(origin, new Vector3(10, 10, 0));
        AssertIsSensibleShot(origin, new Vector3(5, 5, 0));
        AssertIsSensibleShot(origin, new Vector3(2, 2, 0));
        AssertIsSensibleShot(origin, new Vector3(10, 0, 0));
        AssertIsSensibleShot(origin, new Vector3(-10, 0, 0));
        AssertIsSensibleShot(origin, new Vector3(0, -10, 0));
        AssertIsSensibleShot(origin, new Vector3(1, 1, 0));
        AssertIsSensibleShot(origin, new Vector3(.1f, .1f, 0));
    }

    private void AssertIsSensibleShot(Vector3 origin, Vector3 target)
    {
        Vector3? shot = Projectile.CalculateFireVector(origin, target);
        Assert.IsNotNull(shot, $"No vector could be calculated from {origin} to {target}");
        Assert.IsTrue(shot.Value.Z >= 0, $"Shot {shot} is not arcing");
        Assert.IsTrue(
            (MathF.Abs(shot.Value.X) + MathF.Abs(shot.Value.Y)) / MathF.Abs(shot.Value.Z) > .05f,
            $"Shot ${shot} to {target} is excessively arcing");
        Projectile proj = new Projectile(origin, 0f, shot.Value);
        Assert.IsTrue(proj.TimeWillLand < 3f, $"Shot will take projectile {proj.TimeWillLand}s to land");
        TH.AssertIsApproximately(target, proj.FinalPosition);
    }
}
