using System.ComponentModel;
using System.Numerics;
using Navigation;

namespace Tests;

[TestClass]
public class NavGridTests
{
    [TestMethod]
    public void NavGrid_NoPath()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 0, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(1, 4), traversable);
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
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 0), traversable);
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
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(2, 4), traversable);
        Assert.AreEqual(6, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
        Assert.AreEqual(new Vector2Int(0, 1), path[1]);
        Assert.AreEqual(new Vector2Int(0, 2), path[2]);
        Assert.AreEqual(new Vector2Int(0, 3), path[3]);
        Assert.AreEqual(new Vector2Int(1, 4), path[4]);
        Assert.AreEqual(new Vector2Int(2, 4), path[5]);
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
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 1), traversable);
        Assert.AreEqual(2, path.Count);
        Assert.AreEqual(new Vector2Int(0, 0), path[0]);
        Assert.AreEqual(new Vector2Int(0, 1), path[1]);
    }

    [TestMethod]
    public void NavGrid_OopsOnlyY()
    {
        short[,] traversable = new short[,]
        {
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
        };
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(0, 9), traversable);
        Assert.AreEqual(10, path.Count);
    }

    [TestMethod]
    public void NavGrid_OopsOnlyX()
    {
        short[,] traversable = new short[,]
        {
            {0},{1},{1},{1},{1},{1},{1},{1},{1},{0},
        };
        List<Vector2Int> path = NavGrid.FindPath(new Vector2Int(0, 0), new Vector2Int(9, 0), traversable);
        Assert.AreEqual(10, path.Count);
    }

    [TestMethod]
    public void NavGrid_Indexing()
    {
        short[,] traversable = new short[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 1 },
            { 1, 1, 0, 1, 1 }
        };
        Assert.AreEqual(new Vector2Int(2, 4), NavGrid.GetPosition(14, traversable.GetLength(0)));
        Assert.AreEqual(14, NavGrid.GetIndex(new Vector2Int(2, 4), traversable.GetLength(0)));
        Assert.AreEqual(6, NavGrid.GetIndex(new Vector2Int(0, 2), traversable.GetLength(0)));
        Assert.AreEqual(new Vector2Int(0, 2), NavGrid.GetPosition(6, traversable.GetLength(0)));
    }
}