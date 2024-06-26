using System.Numerics;
using Navigation;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Map
{
    public TileType[,] Tiles { get; private set; } = new TileType[0, 0];
    public short[,] Traversable { get; private set; } = new short[0, 0];
    public SpacialPartitioning.Grid Grid { get; private set; } = new SpacialPartitioning.Grid(0, 0);
    public List<Bastion> Bastions { get; private set; } = new();
    public List<Soldier> Soldiers { get; private set; } = new();
    public Dictionary<V2Int, ulong> BastionLands { get; private set; } = new();
    public Dictionary<V2Int, string?> Words { get; private set; } = new();
    private Dictionary<ulong, Dictionary<ulong, List<V2Int>>> bastionPaths = new();
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
        foreach (Bastion bastion in Bastions)
        {
            bastion.Update(deltaTime);
        }

        for (int i = 0; i < Soldiers.Count; i++)
        {
            Soldiers[i].Update(deltaTime);
        }
    }

    public List<V2Int>? GetPathBetweenBastions(ulong startId, ulong endId)
    {
        if (!bastionPaths.ContainsKey(startId) || !bastionPaths[startId].ContainsKey(endId))
        {
            return null;
        }

        return bastionPaths[startId][endId];
    }

    public Vector2? GetNextPathPoint(ulong originId, ulong targetId, int progress)
    {
        List<V2Int>? path = GetPathBetweenBastions(originId, targetId);
        if (path == null || progress + 1 >= path.Count)
        {
            return null;
        }

        return new Vector2(path[progress + 1].X + .5f, path[progress + 1].Y + .5f);
    }

    private void CalculateBastionPathing()
    {
        foreach (Bastion bastion in Bastions)
        {
            ushort[,] pathMap = NavGrid.GetPathMap(V2Int.From(Grid.GetEntityPosition(bastion.Id)), Traversable);
            V2Int sourcePos = V2Int.From(Grid.GetEntityPosition(bastion.Id));

            foreach (Bastion other in Bastions)
            {
                if (bastion.Id == other.Id)
                {
                    continue;
                }

                V2Int targetPos = V2Int.From(Grid.GetEntityPosition(other.Id));
                List<V2Int> path = NavGrid.ReconstructPath(sourcePos, targetPos, pathMap);
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
        Dictionary<ulong, V2Int> locations = new();
        foreach (Bastion bastion in Bastions)
        {
            V2Int? location = Grid.GetEntityGridPos(bastion.Id);
            if (location != null)
            {
                locations.Add(bastion.Id, location.Value);
            }
        }

        BastionLands = Ownership.Calculate(Width, Height, locations);
    }

    private void CalculateValidWordPositions()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Traversable[x, y] == Constants.TRAVERSABLE)
                {
                    Words[new V2Int(x, y)] = null;
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
        foreach (V2Int pos in Words.Keys)
        {
            if (Words[pos] == null)
            {
                placement -= 1;
                if (placement < 0)
                {
                    Words[pos] = "Hello";
                    return;
                }
            }
        }
    }

    public void AttackBastion(int sourceIndex, int targetIndex, SoldierType? type = null, float percent = 1f)
    {
        if (sourceIndex < 0 || sourceIndex >= Bastions.Count
            || targetIndex < 0 || targetIndex >= Bastions.Count)
        {
            return;
        }

        Bastion source = Bastions[sourceIndex];
        ulong targetId = Bastions[targetIndex].Id;

        source.SetDeploymentOrder(targetId, type, percent);
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
        Grid = new SpacialPartitioning.Grid(terrain[0].Length, terrain.Length);
        for (int y = 0; y < terrain.Length; y++)
        {
            for (int x = 0; x < terrain[0].Length; x++)
            {
                switch (terrain[y][x])
                {
                    case 'A':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Constants.BLOCKED;
                        Bastions.Add(new(this, SoldierType.Archer, alliance: ownership[y][x] - '0'));
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            Bastions[^1].Id,
                            Bastion.Radius
                        ));
                        break;
                    case 'W':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Constants.BLOCKED;
                        Bastions.Add(new(this, SoldierType.Warrior, alliance: ownership[y][x] - '0'));
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            Bastions[^1].Id,
                            Bastion.Radius
                        ));
                        break;
                    case 'T':
                        Tiles[x, y] = TileType.Tree;
                        Traversable[x, y] = Constants.BLOCKED;
                        break;
                    case 'X':
                        Tiles[x, y] = TileType.Water;
                        Traversable[x, y] = Constants.BLOCKED;
                        break;
                    default:
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Constants.TRAVERSABLE;
                        break;
                }
            }
        }
    }
}