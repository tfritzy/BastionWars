using System.Numerics;
using BastionWars;

namespace Tests;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Map_PlacesBastions()
    {
        Map map = new(50, 30);
        Assert.IsTrue(map.Bastions.Count > 0);

        foreach (var bastion in map.Bastions)
        {
            Vector2 pos = map.Grid.GetEntityPosition(bastion.Id);
            Assert.IsTrue(map.Travelable[(int)pos.X, (int)pos.Y] == 1);
        }
    }

    [TestMethod]
    public void Map_AllBastionsCanBeReached()
    {
        Map map = new(50, 30);
        int unreachableCount = 0;
        foreach (var bastion in map.Bastions)
        {
            foreach (var other in map.Bastions)
            {
                if (bastion == other)
                {
                    continue;
                }

                Vector2Int start = Vector2Int.From(map.Grid.GetEntityPosition(bastion.Id));
                Vector2Int end = Vector2Int.From(map.Grid.GetEntityPosition(other.Id));
                var path = map.FindPathBetweenBastions(start, end);
                if (path == null || path.Length == 0)
                {
                    unreachableCount++;
                    break;
                }
            }
        }

        Assert.AreEqual(0, unreachableCount, $"{unreachableCount}/{map.Bastions.Count} bastions are unreachable");
    }
}