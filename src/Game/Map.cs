using System.Numerics;
using System.Text;
using SpacialPartitioning;

namespace BastionWars;

public class Map
{
    public TileType[,] Tiles { get; private set; }
    public short[,] Travelable { get; private set; }
    public Grid Grid { get; private set; }
    public List<Bastion> Bastions { get; private set; }

    public Map(int width, int height)
    {
        Bastions = new();
        Grid = new Grid(width, height);
        Tiles = new TileType[width, height];
        Travelable = new short[width, height];
        GenerateTerrain();
        PlaceBastions();
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
        int bastionCount = 8;
        Bastions = new List<Bastion>(bastionCount);

        for (int i = 0; i < bastionCount; i++)
        {
            BastionType type = (BastionType)(i % 3);
            Bastion bastion = new(type);
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
}