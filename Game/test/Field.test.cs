using System.Numerics;
using KeepLordWarriors;
using Schema;
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

    [TestMethod]
    public void Field_InitialStateOnlyContainsOwnedFields()
    {
        Game game = new(TH.GetGameSettings());
        var p = TH.AddPlayer(game);
        var initialState = p.MessageQueue.First(m => m.InitialState != null).InitialState;

        foreach (Field field in game.Map.Fields.Values)
        {
            Keep owner = game.Map.Keeps[game.Map.KeepLands[field.Position]];
            if (owner.OwnerId == p.Id)
            {
                Assert.IsTrue(
                    initialState.GrownFields
                        .Any(f => f.GridPos.X == field.Position.X && f.GridPos.Y == field.Position.Y)
                );
            }
            else
            {
                Assert.IsFalse(
                    initialState.GrownFields
                        .Any(f => f.GridPos.X == field.Position.X && f.GridPos.Y == field.Position.X)
                );
            }
        }
    }

    [TestMethod]
    public void Field_InitialStateHasInitialGrownFields()
    {
        Game game = new(TH.GetGameSettings());
        Field f1 = game.Map.Fields.Values.First();
        f1.RemainingGrowthTime = 1f;
        Field f2 = game.Map.Fields.Values.Last();
        f2.HandleKeystroke(f2.Text[0]);
        var p = TH.AddPlayer(game);
        var initialStates = p.MessageQueue.Where(m => m.InitialState != null).ToList();
        var state = initialStates[0].InitialState;

        Assert.IsTrue(state.GrownFields.Count > 0);
        foreach (Vector2Int pos in game.Map.Fields.Keys)
        {
            if (game.Map.Keeps[game.Map.KeepLands[pos]].OwnerId != p.Id)
            {
                continue;
            }

            if (game.Map.Fields.TryGetValue(pos, out Field? field))
            {
                GrownField? stateField = state.GrownFields.FirstOrDefault(
                    f => f.GridPos.X == pos.X && f.GridPos.Y == pos.Y);

                if (field.RemainingGrowthTime > 0)
                {
                    Assert.IsNull(stateField);
                }
                else
                {
                    Assert.AreEqual(field!.Text, stateField!.Text);
                    Assert.AreEqual(field.TypedIndex, stateField.Progress);
                }
            }
        }
    }

}