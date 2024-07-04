using Godot;
using KeepLordWarriors;
using SpacialPartitioning;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class MapMono : TileMap
{
    private Map Map;
    public string TileSetPath = "res://Configs/tile_set.tres";
    public Godot.Vector2 Center => CalculateCenter();

    private Dictionary<V2Int, Typeable> words = new();

    public MapMono(Map map)
    {
        Map = map;
        words = new();
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

    public override void _Process(double delta)
    {
        SyncWods();
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

    private void SyncWods()
    {
        foreach (var pos in Map.Words.Keys)
        {
            var word = Map.Words[pos];
            if (word != null && !words.ContainsKey(pos))
            {
                words[pos] = new(word);
                words[pos].Position = ToGlobal(MapToLocal(new Vector2I(pos.X, pos.Y)));
                AddChild(words[pos]);
            }
        }
    }

    private Godot.Vector2 CalculateCenter()
    {
        var rect = GetUsedRect();

        GD.Print("Rect: " + rect);

        var center_x = Position.X + (rect.Size.X / 2);
        var center_y = Position.Y + (rect.Size.Y / 2);

        GD.Print("Center: " + center_x + ", " + center_y);

        return new Godot.Vector2(center_x, center_y);
    }
}