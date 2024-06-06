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
    public void Grid_AddsEntitiesToCorrectPartition()
    {
        Grid grid = new(30, 20);
        grid.AddEntity(TH.BuildEntity(3.5f, 6.5f));
    }
}