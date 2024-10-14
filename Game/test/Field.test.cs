using System.Numerics;
using KeepLordWarriors;
using TestHelpers;

namespace Tests;

[TestClass]
public class FieldTests
{
    [TestMethod]
    public void Field_IncrementsProgress()
    {
        Game game = new(TH.GetGameSettings());
        Field field = new(game: game, position: new Vector2Int(0, 0));
        Assert.AreEqual(0, field.TypedIndex);
        field.HandleKeystroke(field.Text[0]);
        Assert.AreEqual(1, field.TypedIndex);
        field.HandleKeystroke('.');
        Assert.AreEqual(1, field.TypedIndex);
        field.HandleKeystroke(field.Text[1]);
        Assert.AreEqual(2, field.TypedIndex);
    }

    [TestMethod]
    public void Field_ReportingNewlyGrown()
    {
        Game game = new(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        Field field = new(game: game, position: new Vector2Int(0, 0));

        // doesn't report initially.
        TH.UpdateGame(game, Game.NetworkTickTime);
        Assert.AreEqual(0, p.MessageQueue.Where(m => m.NewGrownFields != null).ToList().Count);

        field.Reset();
        field.RemainingGrowthTime = 0;

        // doesn't multi report
        field.Update();
        TH.UpdateGame(game, Game.NetworkTickTime);
        field.Update();
        TH.UpdateGame(game, Game.NetworkTickTime);
        Assert.AreEqual(1, p.MessageQueue.Where(m => m.NewGrownFields != null).ToList().Count);
    }
}