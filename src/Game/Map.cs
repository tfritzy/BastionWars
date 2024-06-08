using System.Numerics;
using AStar;
using AStar.Options;
using SpacialPartitioning;

namespace BastionWars;

public class Map
{
    public TileType[,] Tiles { get; private set; }
    public short[,] Travelable { get; private set; }
    public Grid Grid { get; private set; }
    public List<Bastion> Bastions { get; private set; }
    public PathFinder PathFinder { get; private set; }

    public Map(int width, int height)
    {
        Bastions = new();
        Grid = new Grid(width, height);
        Tiles = new TileType[width, height];
        Travelable = new short[width, height];
        GenerateTerrain();
        PlaceBastions();

        var pathfinderOptions = new PathFinderOptions
        {
            PunishChangeDirection = true,
            UseDiagonals = true,
        };
        var worldGrid = new WorldGrid(Travelable);
        PathFinder = new PathFinder(worldGrid, pathfinderOptions);
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

    public Vector2Int[] FindPathBetweenBastions(Vector2Int start, Vector2Int end)
    {
        Travelable[start.X, start.Y] = 1;
        Travelable[end.X, end.Y] = 1;
        var path = PathFinder.FindPath(start, end);
        Travelable[start.X, start.Y] = 0;
        Travelable[end.X, end.Y] = 0;
        return path;
    }
}