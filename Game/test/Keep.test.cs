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
        TH.ErradicateAllArchers(game);
        game.Map.KeepAt(0).SetCount(archers: 10);

        TH.UpdateGame(game, 5f);

        Assert.AreEqual(0, game.Map.Projectiles.Count);
        game.Map.AddSoldier(
            TH.BuildEnemySoldier(Schema.SoldierType.Warrior, keep.Alliance, game),
            game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));

        TH.UpdateGame(game, 1);
        Assert.AreEqual(10, game.Map.Projectiles.Count);
    }


    [TestMethod]
    public void Keep_SendsANetworkUpdateWhenCaptured()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        Keep keep = game.Map.KeepAt(0);
        TH.AddPlayer(game);
        TH.UpdateGame(game, 5f);

        var player = game.Players.Values.First();
        player.MessageQueue.Clear();
        keep.Capture(5);
        TH.UpdateGame(game, Game.NetworkTickTime + .0001f);

        var tileUpdates = player.MessageQueue.Where(m => m.RenderTileUpdates != null).First();
        Assert.AreEqual(12, tileUpdates.RenderTileUpdates.RenderTileUpdates_.Count);

        var keepUpdates = player.MessageQueue.Where(m => m.KeepUpdates != null).ToList();
        Assert.AreEqual(1, keepUpdates.Count);
        Assert.AreEqual(1, keepUpdates.First().KeepUpdates.KeepUpdates.Count); // lol
        Assert.IsTrue(keepUpdates.First().KeepUpdates.KeepUpdates.First().Alliance == 5);
    }

    [TestMethod]
    public void Keep_NetworkTickListsNewProjectiles()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        TH.AddPlayer(game);
        Keep keep = game.Map.KeepAt(0);
        game.Map.KeepAt(0).SetCount(archers: 100);
        game.Map.AddSoldier(
            TH.BuildEnemySoldier(Schema.SoldierType.Warrior, keep.Alliance, game),
            game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));

        TH.UpdateGame(game, (int)(1f / Game.NetworkTickTime));
        int originalProjCount = game.Map.Projectiles.Count;
        Assert.IsTrue(originalProjCount > 0);
        game.Players.Values.First().MessageQueue.Clear();

        TH.UpdateGame(game, Game.NetworkTickTime * .5f);
        var projMsgs = game.Players.Values.First().MessageQueue.Where(m => m.NewProjectiles != null).ToList();
        Assert.AreEqual(0, projMsgs.Count);

        TH.UpdateGame(game, Constants.ArcherBaseCooldown + .1f);
        projMsgs = game.Players.Values.First().MessageQueue.Where(m => m.NewProjectiles != null).ToList();
        Assert.AreEqual(1, projMsgs.Count);
        Assert.IsTrue(projMsgs.First().NewProjectiles.Projectiles.Count >= 100);
    }

    [TestMethod]
    public void Keep_ProjectilesDespawnAfterLanding()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        TH.AddPlayer(game);
        Keep keep = game.Map.KeepAt(0);
        game.Map.KeepAt(0).SetCount(archers: 100);
        game.Map.AddSoldier(
            TH.BuildEnemySoldier(Schema.SoldierType.Warrior, keep.Alliance, game),
            game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(4));
        TH.UpdateGame(game, 1f);
        var arrow = game.Map.Projectiles.First();

        TH.UpdateGame(game, arrow.TimeWillLand - 1f - .1f);
        Assert.IsTrue(game.Map.Projectiles.Contains(arrow));
        TH.UpdateGame(game, .2f);
        Assert.IsFalse(game.Map.Projectiles.Contains(arrow));
    }

    [TestMethod]
    public void Keep_CanShootEverywhereInItsRange()
    {
        Vector3 origin = new Vector3(0, 0, 0);
        AssertIsSensibleShot(origin, new Vector3(7, 7, 0));
        AssertIsSensibleShot(origin, new Vector3(5, 5, 0));
        AssertIsSensibleShot(origin, new Vector3(2, 2, 0));
        AssertIsSensibleShot(origin, new Vector3(10, 0, 0));
        AssertIsSensibleShot(origin, new Vector3(-10, 0, 0));
        AssertIsSensibleShot(origin, new Vector3(0, -10, 0));
        AssertIsSensibleShot(origin, new Vector3(1, 1, 0));
        AssertIsSensibleShot(origin, new Vector3(.1f, .1f, 0));
    }

    [TestMethod]
    public void Keep_SimulateProjectile()
    {
        Vector3 origin = new Vector3(8, 5, 0);
        List<Vector3> targets = new List<Vector3> {
            new Vector3(1f, 3f, 0),
            new Vector3(10f, 0, 0),
            new Vector3(1f, 1f, 0),
            new Vector3(6f, 2f, 0),
            new Vector3(6f, 8f, 0),
            new Vector3(9f, 8f, 0),
            new Vector3(9f, 1f, 0),
            new Vector3(0f, .5f, 0),
            new Vector3(4.499439f, 6.81507f, 0)
        };
        var timeStep = .005f;

        foreach (var target in targets)
        {
            Vector3? shot = Projectile.CalculateFireVector(origin, target);
            Assert.IsNotNull(shot, $"Couldn't find a shot for {target}");
            Projectile p = new Projectile(origin, 0, shot.Value, 1, 1);

            Vector3 pos = origin;
            Vector3 velocity = p.InitialVelocity;
            while (true)
            {
                pos += velocity * timeStep;
                velocity.Z -= Constants.ArrowGravity * timeStep;
                if (pos.Z <= 0)
                {
                    break;
                }
            }

            TH.AssertIsApproximately(pos, target);
        }
    }

    [TestMethod]
    public void Keep_KillsSoldiersArrowsHit()
    {
        Game game = new(TH.GetGameSettings(mode: GenerationMode.Word));
        TH.AddPlayer(game);
        Keep keep = game.Map.KeepAt(0);
        TH.ErradicateAllArchers(game);
        game.Map.KeepAt(0).SetCount(archers: 100);
        var enemy = TH.BuildEnemySoldier(SoldierType.Warrior, keep.Alliance, game);
        var ally = TH.BuildAllySoldier(SoldierType.Warrior, keep.Alliance, game);
        enemy.Freeze();
        ally.Freeze();
        game.Map.AddSoldier(enemy, game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));
        game.Map.AddSoldier(ally, game.Map.Grid.GetEntityPosition(keep.Id) + new Vector2(3));

        TH.UpdateGame(game, .5f);
        var proj = game.Map.Projectiles.First();

        Assert.IsTrue(game.Map.Soldiers.ContainsKey(enemy.Id));
        TH.UpdateGame(game, proj.TimeWillLand - proj.BirthTime + .3f);
        Assert.IsFalse(game.Map.Soldiers.ContainsKey(enemy.Id));
        Assert.IsTrue(game.Map.Soldiers.ContainsKey(ally.Id));
    }

    private void AssertIsSensibleShot(Vector3 origin, Vector3 target)
    {
        Vector3? shot = Projectile.CalculateFireVector(origin, target);
        Assert.IsNotNull(shot, $"No vector could be calculated from {origin} to {target}");
        Assert.IsTrue(shot.Value.Z >= 0, $"Shot {shot} is not arcing");
        Assert.IsTrue(
            (MathF.Abs(shot.Value.X) + MathF.Abs(shot.Value.Y)) / MathF.Abs(shot.Value.Z) > .05f,
            $"Shot ${shot} to {target} is excessively arcing");
        Projectile proj = new Projectile(origin, 0f, shot.Value, 0, Constants.ArrowBaseDamage);
        Assert.IsTrue(proj.TimeWillLand < 3f, $"Shot will take projectile {proj.TimeWillLand}s to land");
        TH.AssertIsApproximately(target, proj.FinalPosition);
    }
}
