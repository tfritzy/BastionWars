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
            Assert.IsFalse(map.Travelable[(int)pos.X, (int)pos.Y]);
        }
    }
}