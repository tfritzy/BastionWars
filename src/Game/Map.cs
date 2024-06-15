using System.Numerics;
using System.Text;
using Navigation;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Map
{
    public TileType[,] Tiles { get; private set; }
    public short[,] Travelable { get; private set; }
    public Grid Grid { get; private set; }
    public List<Bastion> Bastions { get; private set; }
    public List<Soldier> Soldiers { get; private set; } = new();
    private Dictionary<ulong, Dictionary<ulong, List<Vector2Int>>> bastionPaths = new();

    public Map(int width, int height)
    {
        Bastions = new();
        Grid = new Grid(width, height);
        Tiles = new TileType[width, height];
        Travelable = new short[width, height];
        GenerateTerrain();
        PlaceBastions();
        CalculateBastionMaps();
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

    private void GenerateTerrain()
    {
        for (int x = 0; x < Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                Tiles[x, y] = TileType.Land;
                Travelable[x, y] = 1;
            }
        }
    }

    private void PlaceBastions()
    {
        int bastionCount = 4;
        Bastions = new List<Bastion>(bastionCount);

        for (int i = 0; i < bastionCount; i++)
        {
            SoldierType type = (SoldierType)(i % 3);
            Bastion bastion = new(this, type);
            Bastions.Add(bastion);
            Vector2Int pos = GetBuildableTile();
            Grid.AddEntity(new SpacialPartitioning.Entity(
                new Vector2(pos.X, pos.Y),
                bastion.Id,
                Bastion.Radius
            ));
            Travelable[pos.X, pos.Y] = 0;
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

    private void CalculateBastionMaps()
    {
        foreach (Bastion bastion in Bastions)
        {
            ushort[,] pathMap = NavGrid.GetPathMap(Vector2Int.From(Grid.GetEntityPosition(bastion.Id)), Travelable);
            Vector2Int sourcePos = Vector2Int.From(Grid.GetEntityPosition(bastion.Id));

            foreach (Bastion other in Bastions)
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

    private Vector2Int GetBuildableTile()
    {
        Random random = new();
        int x, y;
        do
        {
            x = random.Next(Tiles.GetLength(0));
            y = random.Next(Tiles.GetLength(1));
        } while (Travelable[x, y] == 0);

        return new Vector2Int(x, y);
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
                else if (Travelable[x, y] == 1)
                {
                    sb.Append(' ');
                    continue;
                }
                else
                {
                    sb.Append(Travelable[x, y] == 1 ? ' ' : 'X');
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
}