
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
}