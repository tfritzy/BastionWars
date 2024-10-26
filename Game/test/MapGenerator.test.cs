using KeepLordWarriors;
using TestHelpers;

namespace Tests;

[TestClass]
public class MapGeneratorTests
{
    [TestMethod]
    public void MapGenerator_HasCorrectDimensions()
    {
        string mapStr = MapGenerator.Generate(64, 32);
        Game game = new(TH.GetGameSettings(map: mapStr));
        Console.Write(mapStr);
        Assert.Fail();
    }
}