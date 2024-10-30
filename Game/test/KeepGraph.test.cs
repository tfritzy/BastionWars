
using System.Diagnostics.Tracing;
using KeepLordWarriors;
using Navigation;
using TestHelpers;

namespace Tests;

[TestClass]
public class KeepGraphTests
{
    [TestMethod]
    public void KeepGraph_CreatesAppropriateConnections()
    {
        string map = MapGenerator.Generate(32, 64);
        Game game = new Game(TH.GetGameSettings(map: map));
        Dictionary<uint, KeepGraph.Node> graph = KeepGraph.Calculate(game.Map.KeepLands);

        foreach (KeepGraph.Node node in graph.Values)
        {
            Assert.IsTrue(node.Next.Count > 0);
            foreach (KeepGraph.Node n in node.Next)
            {
                // All nodes are reflexive.
                Assert.IsTrue(graph[n.KeepId].Next.Any(prev => prev.KeepId == node.KeepId));
            }
        }
    }

    [TestMethod]
    public void KeepGraph_CalculatesDistancesCorrectly()
    {
        //            1   1   1   2   2 2 3      
        string map = "W...A...A...W...W.A.A\n.....................";
        Game game = new Game(TH.GetGameSettings(map: map));
        game.Map.KeepAt(0).Capture(1, null);
        game.Map.KeepAt(1).Capture(1, null);
        game.Map.KeepAt(2).Capture(1, null);
        game.Map.KeepAt(3).Capture(2, null);
        game.Map.KeepAt(4).Capture(2, null);
        game.Map.KeepAt(5).Capture(2, null);
        game.Map.KeepAt(6).Capture(3, null);

        int[] expectedDistances = [2, 1, 0, 0, 1, 0, 0];
        for (int i = 0; i < expectedDistances.Length; i++)
        {
            Assert.AreEqual(expectedDistances[i], game.Map.Graph[game.Map.KeepAt(i).Id].DistanceFromFrontline);
        }

        //               1   1   2   2   2 2 3      
        // string map = "W...A...A...W...W.A.A";
        game.Map.KeepAt(2).Capture(2, null);

        expectedDistances = [1, 0, 0, 1, 1, 0, 0];
        for (int i = 0; i < expectedDistances.Length; i++)
        {
            Assert.AreEqual(expectedDistances[i], game.Map.Graph[game.Map.KeepAt(i).Id].DistanceFromFrontline);
        }
    }
}