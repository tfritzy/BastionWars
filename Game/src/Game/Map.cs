using System.Numerics;
using Navigation;
using Schema;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Map
{
    public TileType[,] Tiles { get; private set; } = new TileType[0, 0];
    public RenderTType[,] RenderTiles { get; private set; } = new RenderTType[0, 0];
    public short[,] Traversable { get; private set; } = new short[0, 0];
    public Grid Grid { get; private set; } = new(0, 0);
    public Dictionary<ulong, Keep> Keeps { get; private set; } = new();
    public List<Soldier> Soldiers { get; private set; } = new();
    public Dictionary<Vector2Int, ulong> KeepLands { get; private set; } = new();
    public Dictionary<Vector2Int, Word?> Words { get; private set; } = new();
    private Dictionary<ulong, Dictionary<ulong, List<Vector2Int>>> bastionPaths = new();
    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    public Map(string rawMap)
    {
        ParseMap(rawMap);
        CalculateBastionPathing();
        CalculateBastionOwnership();
        CalculateValidWordPositions();
    }

    public void Update(double deltaTime)
    {
        foreach (Keep bastion in Keeps.Values)
        {
            bastion.Update(deltaTime);
        }

        for (int i = 0; i < Soldiers.Count; i++)
        {
            Soldiers[i].Update(deltaTime);
        }
    }

    public List<Vector2Int>? GetPathBetweenBastions(ulong startId, ulong endId)
    {
        if (!bastionPaths.ContainsKey(startId) || !bastionPaths[startId].ContainsKey(endId))
        {
            return null;
        }

        return bastionPaths[startId][endId];
    }

    public Vector2? GetNextPathPoint(ulong originId, ulong targetId, int progress)
    {
        List<Vector2Int>? path = GetPathBetweenBastions(originId, targetId);
        if (path == null || progress + 1 >= path.Count)
        {
            return null;
        }

        return new Vector2(path[progress + 1].X + .5f, path[progress + 1].Y + .5f);
    }

    private void CalculateBastionPathing()
    {
        foreach (Keep bastion in Keeps.Values)
        {
            ushort[,] pathMap = NavGrid.GetPathMap(Vector2Int.From(Grid.GetEntityPosition(bastion.Id)), Traversable);
            Vector2Int sourcePos = Vector2Int.From(Grid.GetEntityPosition(bastion.Id));

            foreach (Keep other in Keeps.Values)
            {
                if (bastion.Id == other.Id)
                {
                    continue;
                }

                Vector2Int targetPos = Vector2Int.From(Grid.GetEntityPosition(other.Id));
                List<Vector2Int> path = NavGrid.ReconstructPath(sourcePos, targetPos, pathMap);
                if (path == null)
                {
                    continue;
                }

                if (!bastionPaths.ContainsKey(bastion.Id))
                {
                    bastionPaths.Add(bastion.Id, new());
                }

                bastionPaths[bastion.Id].Add(other.Id, path);
            }
        }
    }

    private void CalculateBastionOwnership()
    {
        Dictionary<ulong, Vector2Int> locations = new();
        foreach (Keep bastion in Keeps.Values)
        {
            Vector2Int? location = Grid.GetEntityGridPos(bastion.Id);
            if (location != null)
            {
                locations.Add(bastion.Id, location.Value);
            }
        }

        KeepLands = Ownership.Calculate(Width, Height, locations);
    }

    private void CalculateValidWordPositions()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Traversable[x, y] == Navigation.Constants.TRAVERSABLE && x % 2 == 0 && y % 2 == 0)
                {
                    Words[new Vector2Int(x, y)] = null;
                }
            }
        }
    }

    public void PlaceWord()
    {
        int numOpenSpots = Words.Values.Count(w => w == null);
        if (numOpenSpots == 0)
            return;

        int placement = new Random().Next(0, numOpenSpots);
        foreach (Vector2Int pos in Words.Keys)
        {
            if (Words[pos] == null)
            {
                placement -= 1;
                if (placement < 0)
                {
                    string word = GetRandomWord();
                    Words[pos] = new Word(word, pos);
                    return;
                }
            }
        }
    }

    public static string GetRandomWord()
    {
        return Dictionary.MostCommon[Randy.Random.Next(0, Dictionary.MostCommon.Length)];
    }

    public void AddSoldier(Soldier soldier, Vector2 pos)
    {
        Soldiers.Add(soldier);
        Grid.AddEntity(new SpacialPartitioning.Entity(
            pos,
            soldier.Id,
            Soldier.Radius
        ));
    }

    public void RemoveSoldier(ulong id)
    {
        var removed = Soldiers.RemoveAll((s) => s.Id == id);
        Grid.RemoveEntity(id);
    }

    public float MeleePowerOf(Soldier soldier) => MeleePowerOf(soldier.Type, soldier.Alliance);
    public float MeleePowerOf(SoldierType soldierType, int alliance)
    {
        return soldierType switch
        {
            SoldierType.Archer => 1,
            SoldierType.Warrior => 4,
            _ => 0
        };
    }

    public Keep KeepAt(int index)
    {
        return Keeps.Values.ElementAt(index);
    }

    private void ParseMap(string rawMap)
    {
        rawMap = rawMap.Replace("\r", "");
        rawMap = rawMap.Replace(" ", "");
        string[] pieces = rawMap.Split("</>");
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i] = pieces[i].Trim();
        }
        string[] terrain = pieces[0].Split("\n");
        string[] ownership = pieces[1].Split("\n");
        string[] elevation = pieces[2].Split("\n");

        Tiles = new TileType[terrain[0].Length, terrain.Length];
        Traversable = new short[terrain[0].Length, terrain.Length];
        Grid = new Grid(terrain[0].Length, terrain.Length);
        for (int y = 0; y < terrain.Length; y++)
        {
            for (int x = 0; x < terrain[0].Length; x++)
            {
                switch (terrain[y][x])
                {
                    case 'A':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        var archerKeep = new Keep(this, SoldierType.Archer, alliance: ownership[y][x] - '0');
                        Keeps.Add(archerKeep.Id, archerKeep);
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            archerKeep.Id,
                            Keep.Radius
                        ));
                        break;
                    case 'W':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        var warriorKeep = new Keep(this, SoldierType.Warrior, alliance: ownership[y][x] - '0');
                        Keeps.Add(warriorKeep.Id, warriorKeep);
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            warriorKeep.Id,
                            Keep.Radius
                        ));
                        break;
                    case 'T':
                        Tiles[x, y] = TileType.Tree;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        break;
                    case 'X':
                        Tiles[x, y] = TileType.Water;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        break;
                    default:
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.TRAVERSABLE;
                        break;
                }
            }
        }

        ParseRenderTiles();
    }

    private void ParseRenderTiles()
    {
        RenderTiles = new RenderTType[Width + 1, Height + 1];

        for (int x = -1; x < Width; x++)
        {
            for (int y = -1; y < Height; y++)
            {
                RenderTiles[x + 1, y + 1] = GetRenderTileCase(
                    tl: GetMaybeOOBTile(x, y),
                    tr: GetMaybeOOBTile(x + 1, y),
                    bl: GetMaybeOOBTile(x + 1, y + 1),
                    br: GetMaybeOOBTile(x, y + 1)
                );
            }
        }
    }

    private TileType GetMaybeOOBTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return TileType.Water;
        }

        return Tiles[x, y];
    }

    private RenderTType GetRenderTileCase(
        TileType tl,
        TileType tr,
        TileType bl,
        TileType br
    )
    {
        int tileCase = 0;
        if (tl != TileType.Water)
        {
            tileCase |= 1;
        }

        if (tr != TileType.Water)
        {
            tileCase |= 2;
        }

        if (br != TileType.Water)
        {
            tileCase |= 4;
        }

        if (bl != TileType.Water)
        {
            tileCase |= 8;
        }

        return (RenderTType)tileCase;
    }
}