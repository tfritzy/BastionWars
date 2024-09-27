using System.Numerics;
using System.Text;
using KeepLordWarriors;
using Navigation;
using Schema;
using TestHelpers;

namespace Tests;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void Map_PlacesKeeps()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.ThirtyByTwenty));
        Assert.IsTrue(game.Map.Keeps.Count > 0);

        foreach (var keep in game.Map.Keeps.Values)
        {
            Vector2 pos = game.Map.Grid.GetEntityPosition(keep.Id);
            Assert.IsTrue(game.Map.Traversable[(int)pos.X, (int)pos.Y] == 0);
        }
    }

    [TestMethod]
    public void Map_AllKeepsCanBeNavigatedBetween()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive));
        foreach (var keep in game.Map.Keeps.Values)
        {
            foreach (var other in game.Map.Keeps.Values)
            {
                if (keep == other)
                {
                    continue;
                }

                List<Vector2Int>? path = game.Map.GetPathBetweenKeeps(keep.Id, other.Id);
                Assert.IsTrue(path?.Count > 0);
                Assert.AreEqual(Vector2Int.From(game.Map.Grid.GetEntityPosition(keep.Id)), path[0]);
                Assert.AreEqual(Vector2Int.From(game.Map.Grid.GetEntityPosition(other.Id)), path[^1]);
            }
        }
    }

    [TestMethod]
    public void Map_ReadsKeepsCorrectly()
    {
        Game game = new Game(TH.GetGameSettings(map: TestMaps.TenByFive));
        List<int> expectedAlliances = new() { 2, 1, 1, 1, 1, 3 };
        List<Vector2Int> expectedPositions = new()
        {
            new Vector2Int(1, 0),
            new Vector2Int(3, 0),
            new Vector2Int(2, 2),
            new Vector2Int(0, 3),
            new Vector2Int(2, 4),
            new Vector2Int(4, 4),
        };
        List<SoldierType> expectedTypes = new()
        {
            SoldierType.Warrior,
            SoldierType.Archer,
            SoldierType.Warrior,
            SoldierType.Archer,
            SoldierType.Archer,
            SoldierType.Warrior
        };

        for (int i = 0; i < game.Map.Keeps.Count; i++)
        {
            Assert.AreEqual(expectedAlliances[i], game.Map.Keeps.Values.ToList()[i].Alliance);
            Assert.AreEqual(expectedTypes[i], game.Map.Keeps.Values.ToList()[i].SoldierType);
            Assert.AreEqual(expectedPositions[i], game.Map.Grid.GetEntityGridPos(game.Map.Keeps.Values.ToList()[i].Id));
        }
    }

    [TestMethod]
    public void Map_CalculatesKeepLands()
    {
        string rawMap = @"
            W . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . W . W . . . . .
            </>
            1 . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . . . . . . . . .
            . . 0 . 2 . . . . .
            </>
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0 
            0 0 0 0 0 0 0 0 0 0";
        Game game = new(TH.GetGameSettings(map: rawMap));
        uint b0 = game.Map.KeepAt(0).Id;
        uint b1 = game.Map.KeepAt(1).Id;
        uint b2 = game.Map.KeepAt(2).Id;

        StringBuilder actualOwnership = new();
        Dictionary<uint, string> lookup = new() {
            {b0, "0"}, {b1, "1"}, {b2, "2"},
        };

        for (int y = 0; y < game.Map.Height; y++)
        {
            for (int x = 0; x < game.Map.Width; x++)
            {
                actualOwnership.Append($"{lookup[game.Map.KeepLands[new Vector2Int(x, y)]]}");
                if (x != game.Map.Width - 1)
                    actualOwnership.Append(" ");
            }
            if (y != game.Map.Height - 1)
                actualOwnership.Append($"\n");
        }

        Console.WriteLine(actualOwnership);

        string expectedOwnership =
            "0 0 0 0 0 1 1 2 2 2\n" +
            "0 0 0 0 1 1 2 2 2 2\n" +
            "0 0 0 1 1 2 2 2 2 2\n" +
            "1 1 1 1 2 2 2 2 2 2\n" +
            "1 1 1 1 2 2 2 2 2 2";
        Assert.AreEqual(expectedOwnership, actualOwnership.ToString());
    }

    [TestMethod]
    public void Map_PlacesWords()
    {
        Game game = new(TH.GetGameSettings(map: TestMaps.TenByFive, mode: GenerationMode.Word));
        int numAvailableSpots = 0;
        for (int x = 0; x < game.Map.Width; x++)
        {
            for (int y = 0; y < game.Map.Height; y++)
            {
                if (x % 2 == 0 && y % 2 == 0)
                {
                    numAvailableSpots += game.Map.Traversable[x, y] == Navigation.Constants.TRAVERSABLE ? 1 : 0;
                }
            }
        }
        Assert.AreEqual(numAvailableSpots, game.Map.Words.Keys.Count);
        Assert.AreEqual(Game.InitialWordCount, game.Map.Words.Values.Count(w => w != null));
        game.Map.PlaceWord();
        Assert.AreEqual(Game.InitialWordCount + 1, game.Map.Words.Values.Count(w => w != null));
        game.Map.PlaceWord();
        Assert.AreEqual(Game.InitialWordCount + 2, game.Map.Words.Values.Count(w => w != null));

        for (int i = 0; i < 100; i++)
        {
            game.Map.PlaceWord();
        }

        Assert.AreEqual(numAvailableSpots, game.Map.Words.Values.Count(w => w != null));
    }
}