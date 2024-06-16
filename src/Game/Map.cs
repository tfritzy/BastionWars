using System.Collections;
using System.Numerics;
using System.Text;
using Navigation;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Map
{
    public TileType[,] Tiles { get; private set; } = new TileType[0, 0];
    public short[,] Traversable { get; private set; } = new short[0, 0];
    public Grid Grid { get; private set; } = new Grid(0, 0);
    public List<Bastion> Bastions { get; private set; } = new();
    public List<Soldier> Soldiers { get; private set; } = new();
    public Dictionary<V2Int, ulong> BastionLands { get; private set; } = new();
    private Dictionary<ulong, Dictionary<ulong, List<V2Int>>> bastionPaths = new();
    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    public Map(string rawMap)
    {
        ParseMap(rawMap);
        CalculateBastionPathing();
        CalculateBastionOwnership();
    }

    public void Update(float deltaTime)
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

    private void ParseTerrain(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Tiles[x, y] = map[x, y] == 'X' ? TileType.Water : TileType.Land;
                Traversable[x, y] = map[x, y] == 'X' ? (short)0 : (short)1;
            }
        }
    }

    private void ParseBastions(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (char.IsDigit(map[x, y]))
                {
                    int alliance = int.Parse(map[x, y].ToString());
                    Bastion bastion = new(this, SoldierType.Warrior, alliance: alliance);
                    Bastions.Add(bastion);
                    Grid.AddEntity(new SpacialPartitioning.Entity(
                        new Vector2(x, y),
                        bastion.Id,
                        Bastion.Radius
                    ));
                    Traversable[x, y] = 0;
                }
            }
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

    private V2Int GetBuildableTile()
    {
        Random random = new();
        int x, y;
        do
        {
            x = random.Next(Tiles.GetLength(0));
            y = random.Next(Tiles.GetLength(1));
        } while (Traversable[x, y] == 0);

        return new V2Int(x, y);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int x = 0; x < Tiles.GetLength(0) + 2; x++)
        {
            sb.Append('-');
        }
        sb.AppendLine();

        for (int y = 0; y < Tiles.GetLength(1); y++)
        {
            sb.Append('|');
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                if (Bastions.Any(b => Grid.GetEntityPosition(b.Id) == new Vector2(x, y)))
                {
                    sb.Append('B');
                    continue;
                }
                else if (Traversable[x, y] == 1)
                {
                    sb.Append(' ');
                    continue;
                }
                else
                {
                    sb.Append(Traversable[x, y] == 1 ? ' ' : 'X');
                }
            }

            sb.Append("|");
            sb.AppendLine();
        }

        for (int x = 0; x < Tiles.GetLength(0) + 2; x++)
        {
            sb.Append('-');
        }


        return sb.ToString();
    }

    public void AttackBastion(ulong sourceId, ulong targetId, SoldierType? type = null, float percent = 1f)
    {
        Bastion source = Bastions.First(b => b.Id == sourceId);

        if (source == null || !Bastions.Any(b => b.Id == targetId))
        {
            return;
        }

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
        Console.WriteLine("Soldiers removed: " + removed);
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
        Grid = new Grid(terrain[0].Length, terrain.Length);
        for (int y = 0; y < terrain.Length; y++)
        {
            for (int x = 0; x < terrain[0].Length; x++)
            {
                switch (terrain[y][x])
                {
                    case 'A':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = 0;
                        Bastions.Add(new(this, SoldierType.Archer, alliance: ownership[y][x] - '0'));
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            Bastions[^1].Id,
                            Bastion.Radius
                        ));
                        break;
                    case 'W':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = 0;
                        Bastions.Add(new(this, SoldierType.Warrior, alliance: ownership[y][x] - '0'));
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x, y),
                            Bastions[^1].Id,
                            Bastion.Radius
                        ));
                        break;
                    case 'X':
                        Tiles[x, y] = TileType.Water;
                        Traversable[x, y] = 0;
                        break;
                    default:
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = 1;
                        break;
                }
            }
        }
    }
}