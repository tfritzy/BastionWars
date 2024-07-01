using Godot;
using KeepLordWarriors;
using SpacialPartitioning;
using System;
using System.Numerics;

public partial class MapMono : TileMap
{
    private Map Map;
    public string TileSetPath = "res://Configs/tile_set.tres";
    public Godot.Vector2 Center => CalculateCenter();

    public MapMono(Map map)
    {
        Map = map;
    }

    public override void _Ready()
    {
        TileSet tileSet = GD.Load<TileSet>(TileSetPath);
        if (tileSet == null)
        {
            GD.Print("Failed to load TileSet from path: " + TileSetPath);
            return;
        }

        this.TileSet = tileSet;

        GenerateGrid();
        SpawnBastions();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                TileType type = Map.Tiles[x, y];
                SetCell(0, new Vector2I(x, y), (int)type, new Vector2I(0, 0));
            }
        }
    }

    private void SpawnBastions()
    {
        foreach (var bastion in Map.Bastions)
        {
            BastionMono bastionMono = new(bastion);
            V2Int bastionPos = this.Map.Grid.GetEntityGridPos(bastion.Id).Value;
            bastionMono.Position =
                ToGlobal(MapToLocal(new Vector2I(bastionPos.X, bastionPos.Y)));
            AddChild(bastionMono);
        }
    }

    private Godot.Vector2 CalculateCenter()
    {
        var rect = GetUsedRect();

        var center_x = Position.X + (rect.Size.X / 2)
        var center_y = Position.Y + (rect.Size.Y / 2)

        return new Godot.Vector2(center_x, center_y);
    }
}