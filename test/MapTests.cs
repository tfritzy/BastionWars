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
            Assert.IsTrue(map.Travelable[(int)pos.X, (int)pos.Y] == 0);
        }
    }

    [TestMethod]
    public void Map_AllBastionsCanBeNavigatedBetween()
    {
        Map map = new(5, 5);
        int unreachable = 0;
        foreach (var bastion in map.Bastions)
        {
            foreach (var other in map.Bastions)
            {
                if (bastion == other)
                {
                    continue;
                }

                List<Vector2Int> path = map.GetPathBetweenBastions(bastion.Id, other.Id);
                if (path.Count == 0)
                {
                    unreachable++;
                    Console.WriteLine($"No path between {bastion.Id} and {other.Id}");
                }
                else
                {
                    Console.WriteLine($"Path between {bastion.Id} and {other.Id}: {string.Join(" -> ", path)}");
                }
            }
        }

        Assert.AreEqual(0, unreachable);
    }
}