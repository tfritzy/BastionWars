using KeepLordWarriors;
using TestHelpers;

namespace Tests;

[TestClass]
public class MapGeneratorTests
{
    [TestMethod]
    public void MapGenerator_HasCorrectDimensions()
    {
        Game game = new(TH.GetGameSettings(map: MapGenerator.Generate(64, 32)));
        Console.Write(game.Map);
    }
}