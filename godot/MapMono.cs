using Godot;
using KeepLordWarriors;
using SpacialPartitioning;
using System;
using System.Numerics;

public partial class MapMono : TileMap
{
    private Map Map;
    public string TileSetPath = "res://Configs/tile_set.tres";

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
                SetCell(0, new Vector2I(x, y), 1, new Vector2I(0, 0));
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
}