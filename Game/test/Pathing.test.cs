using System.Numerics;
using Castle.Components.DictionaryAdapter;
using Schema;
using TestHelpers;
using Tests;

namespace KeepLordWarriors;

[TestClass]
public class PathingTests
{
    [TestMethod]
    public void Pathing_CorrectWalkTypesClockwise()
    {
        var walkPath = Pathing.GetWalkTypes(
            new List<Vector2Int>() {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(2, 1),
                new Vector2Int(2, 2),
                new Vector2Int(1, 2),
                new Vector2Int(0, 2),
                new Vector2Int(0, 1),
                new Vector2Int(0, 0)
            }
        );

        Assert.AreEqual(WalkPathType.StraightToRight, walkPath[0]);
        Assert.AreEqual(WalkPathType.StraightToRight, walkPath[1]);
        Assert.AreEqual(WalkPathType.CircularLeftDown, walkPath[2]);
        Assert.AreEqual(WalkPathType.StraightDown, walkPath[3]);
        Assert.AreEqual(WalkPathType.CircularUpLeft, walkPath[4]);
        Assert.AreEqual(WalkPathType.StraightLeft, walkPath[5]);
        Assert.AreEqual(WalkPathType.CircularRightUp, walkPath[6]);
        Assert.AreEqual(WalkPathType.StraightUp, walkPath[7]);
        Assert.AreEqual(WalkPathType.StraightUp, walkPath[8]);

        walkPath = Pathing.GetWalkTypes(
            new List<Vector2Int>() {
                new Vector2Int(1, 0),
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
            }
        );
        Assert.AreEqual(WalkPathType.StraightLeft, walkPath[0]);
        Assert.AreEqual(WalkPathType.CircularRightDown, walkPath[1]);
        Assert.AreEqual(WalkPathType.StraightDown, walkPath[2]);
    }

    [TestMethod]
    public void Pathing_CorrectWalkTypesCounterClockwise()
    {
        var walkPath = Pathing.GetWalkTypes(
            new List<Vector2Int>() {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
                new Vector2Int(2, 2),
                new Vector2Int(2, 1),
                new Vector2Int(2, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 0)
            }
        );

        Assert.AreEqual(WalkPathType.StraightDown, walkPath[0]);
        Assert.AreEqual(WalkPathType.StraightDown, walkPath[1]);
        Assert.AreEqual(WalkPathType.CircularUpRight, walkPath[2]);
        Assert.AreEqual(WalkPathType.StraightToRight, walkPath[3]);
        Assert.AreEqual(WalkPathType.CircularLeftUp, walkPath[4]);
        Assert.AreEqual(WalkPathType.StraightUp, walkPath[5]);
        Assert.AreEqual(WalkPathType.CircularDownLeft, walkPath[6]);
        Assert.AreEqual(WalkPathType.StraightLeft, walkPath[7]);
        Assert.AreEqual(WalkPathType.StraightLeft, walkPath[8]);

        walkPath = Pathing.GetWalkTypes(
            new List<Vector2Int>() {
                new Vector2Int(0, 1),
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
            }
        );
        Assert.AreEqual(WalkPathType.StraightUp, walkPath[0]);
        Assert.AreEqual(WalkPathType.CircularDownRight, walkPath[1]);
        Assert.AreEqual(WalkPathType.StraightToRight, walkPath[2]);
    }
}