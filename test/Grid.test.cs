using System.Numerics;

namespace Tests;

[TestClass]
public class GridTest
{
    [TestMethod]
    public void Grid_MustBeDivisibleByPartitionSize()
    {
        Assert.ThrowsException<ArgumentException>(() => new Grid(3, 10));
        Assert.ThrowsException<ArgumentException>(() => new Grid(10, 3));
        Grid grid = new(30, 20);
    }

    [TestMethod]
    public void Grid_AddEntity()
    {
        Grid grid = new(30, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 0f, 1f);
        var e2 = TH.AddNewEntity(grid, 0f, 0f, 1f);
        Assert.ThrowsException<ArgumentException>(() => grid.AddEntity(e1));
    }

    [TestMethod]
    public void Grid_CollisionChecks()
    {
        Grid grid = new(30, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 20f, 1f);
        var e2 = TH.AddNewEntity(grid, 0f, 22.1f, 1f);

        var collisions = grid.GetCollisions(new Vector2(0f, 19f), 2f);
        Assert.AreEqual(1, collisions.Count);
        Assert.AreEqual(e1.Id, collisions[0]);

        var e3 = TH.AddNewEntity(grid, 0f, 21.8f, 1f);
        var e4 = TH.AddNewEntity(grid, 0f, 16.1f, 1f);
        grid.AddEntity(TH.BuildEntity(0f, 15.9f, 1f));

        collisions = grid.GetCollisions(new Vector2(0f, 19f), 2f);
        Assert.AreEqual(3, collisions.Count);
        CollectionAssert.AreEquivalent(new ulong[] { e1.Id, e3.Id, e4.Id }, collisions.ToArray());
    }

    [TestMethod]
    public void Grid_Collision_EntityOverlappingPartition()
    {
        Grid grid = new(30, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 21f, 3f);
        var collisions = grid.GetCollisions(new Vector2(0f, 17f), 2f);
        Assert.AreEqual(1, collisions.Count);
    }
}