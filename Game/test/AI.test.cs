
using System.Diagnostics.Tracing;
using KeepLordWarriors;
using Navigation;
using TestHelpers;

namespace Tests;

[TestClass]
public class AITests
{
    [TestMethod]
    public void AI_HarvestsAvailableFields()
    {
        Game game = new Game(TH.GetGameSettings(map: MapGenerator.Generate(16, 16)));
        var ai = TH.AddAI(game);

        var grownOwnedFields = TH.OwnedAndGrownFields(game, ai.Id);
        int originalCount = grownOwnedFields.Count;
        Assert.IsTrue(originalCount > 1);

        AI.HarvestFields(game, ai.Id, ai.AIConfig!.BaseHarvestCooldown - .01f);
        Assert.AreEqual(originalCount, TH.OwnedAndGrownFields(game, ai.Id).Count);
        AI.HarvestFields(game, ai.Id, .02f);
        Assert.AreEqual(originalCount - 1, TH.OwnedAndGrownFields(game, ai.Id).Count);
        AI.HarvestFields(game, ai.Id, .02f);
        Assert.AreEqual(originalCount - 1, TH.OwnedAndGrownFields(game, ai.Id).Count);
    }
}