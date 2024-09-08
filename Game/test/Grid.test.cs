using System.Numerics;
using TestHelpers;

namespace Tests;

[TestClass]
public class GridTest
{
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
        CollectionAssert.AreEquivalent(new uint[] { e1.Id, e3.Id, e4.Id }, collisions.ToArray());
    }

    [TestMethod]
    public void Grid_Collision_EntityOverlappingPartition()
    {
        Grid grid = new(30, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 21f, 3f);
        var collisions = grid.GetCollisions(new Vector2(0f, 17f), 2f);
        Assert.AreEqual(1, collisions.Count);
    }

    [TestMethod]
    public void Grid_MovingEntity()
    {
        Grid grid = new(30, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 20f, 1f);
        var e2 = TH.AddNewEntity(grid, 0f, 23f, 1f);

        var collisions = grid.GetCollisions(new Vector2(0f, 19f), 2f);
        Assert.AreEqual(1, collisions.Count);
        Assert.AreEqual(e1.Id, collisions[0]);

        grid.MoveEntity(e1.Id, new Vector2(0f, 25f));
        collisions = grid.GetCollisions(new Vector2(0f, 19f), 4f);
        Assert.AreEqual(1, collisions.Count);
        Assert.AreEqual(e2.Id, collisions[0]);

        grid.MoveEntity(e1.Id, new Vector2(0f, 29.9f));
        collisions = grid.GetCollisions(new Vector2(0f, 33f), 4f);
        Assert.AreEqual(1, collisions.Count);
        Assert.AreEqual(e1.Id, collisions[0]);

        Assert.AreEqual(2, grid.EntityCount);
    }

    [TestMethod]
    public void Grid_MovingEntity_OutOfBounds()
    {
        Grid grid = new(20, 30);
        var e1 = TH.AddNewEntity(grid, 0f, 20f, 1f);

        grid.MoveEntity(e1.Id, new Vector2(-1, 1));
        Assert.AreEqual(new Vector2(0, 1), grid.GetEntityPosition(e1.Id));

        grid.MoveEntity(e1.Id, new Vector2(21, 1));
        Assert.AreEqual(new Vector2(20, 1), grid.GetEntityPosition(e1.Id));

        grid.MoveEntity(e1.Id, new Vector2(1, -1));
        Assert.AreEqual(new Vector2(1, 0), grid.GetEntityPosition(e1.Id));

        grid.MoveEntity(e1.Id, new Vector2(1, 31));
        Assert.AreEqual(new Vector2(1, 30), grid.GetEntityPosition(e1.Id));
    }
}
