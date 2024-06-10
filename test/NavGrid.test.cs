using System.Numerics;
using Navigation;

namespace Tests;

[TestClass]
public class NavGridTests
{
    [TestMethod]
    public void NavGrid_FindPath()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(4, 2));
        Assert.AreEqual(6, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
        Assert.AreEqual(new Vector2Int(1, 0), path[1]);
        Assert.AreEqual(new Vector2Int(2, 0), path[2]);
        Assert.AreEqual(new Vector2Int(3, 0), path[3]);
        Assert.AreEqual(new Vector2Int(4, 1), path[4]);
        Assert.AreEqual(new Vector2Int(4, 2), path[5]);
    }

    [TestMethod]
    public void NavGrid_NoPath()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 0, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(4, 1));
        Assert.AreEqual(0, path.Count);
    }

    [TestMethod]
    public void NavGrid_SameStartEnd()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 0));
        Assert.AreEqual(1, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
    }

    [TestMethod]
    public void NavGrid_CanWorkFromBlockedStartAndEnd()
    {
        short[,] traversable = new short[,]
        {
            { 0, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 0 }
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(4, 2));
        Assert.AreEqual(6, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
        Assert.AreEqual(new Vector2Int(1, 0), path[1]);
        Assert.AreEqual(new Vector2Int(2, 0), path[2]);
        Assert.AreEqual(new Vector2Int(3, 0), path[3]);
        Assert.AreEqual(new Vector2Int(4, 1), path[4]);
        Assert.AreEqual(new Vector2Int(4, 2), path[5]);
    }

    [TestMethod]
    public void NavGrid_NeighborDestionation()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(1, 0));
        Assert.AreEqual(2, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
        Assert.AreEqual(new Vector2Int(1, 0), path[1]);
    }

    [TestMethod]
    public void NavGrid_OopsOnlyX()
    {
        short[,] traversable = new short[,]
        {
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(9, 0));
        Assert.AreEqual(10, path.Count);
    }

    [TestMethod]
    public void NavGrid_OopsOnlyY()
    {
        short[,] traversable = new short[,]
        {
            {0},{1},{1},{1},{1},{1},{1},{1},{1},{0},
        };
        NavGrid grid = new(traversable);
        List<Vector2Int> path = grid.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 9));
        Assert.AreEqual(10, path.Count);
    }
}