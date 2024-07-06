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
    public Dictionary<V2Int, Typeable> Words = new();

    public MapMono(Map map)
    {
        Map = map;
        Words = new();
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
        ColorLands();
    }

    public override void _Process(double delta)
    {
        SyncWords();
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
        foreach (var bastion in Map.Keeps)
        {
            BastionMono bastionMono = new(bastion);
            V2Int bastionPos = this.Map.Grid.GetEntityGridPos(bastion.Id).Value;
            bastionMono.Position =
                ToGlobal(MapToLocal(new Vector2I(bastionPos.X, bastionPos.Y)));
            AddChild(bastionMono);
        }
    }

    private void SyncWords()
    {
        foreach (var pos in Map.Words.Keys)
        {
            var word = Map.Words[pos];
            if (word != null && !Words.ContainsKey(pos))
            {
                Words[pos] = new Typeable(word.Text)
                {
                    Position = ToGlobal(MapToLocal(new Vector2I(pos.X, pos.Y)))
                };
                AddChild(Words[pos]);
            }
        }

        foreach (var pos in Words.Keys)
        {
            var word = Map.Words[pos];
            if (word == null)
            {
                RemoveChild(Words[pos]);
                Words.Remove(pos);
            }
        }
    }

    private void ColorLands()
    {
        Dictionary<ulong, Keep> keeps = new();
        foreach (Keep keep in Map.Keeps)
        {
            keeps.Add(keep.Id, keep);
        }

        foreach (var pos in Map.BastionLands.Keys)
        {
            ulong owner = Map.BastionLands[pos];
            if (keeps.ContainsKey(owner))
            {
                Keep keep = keeps[owner];
                var worldPos = ToGlobal(MapToLocal(new Vector2I(pos.X, pos.Y)));
                var color = GetColor(keep.Alliance);
                var transparentSheet = new ColorRect
                {
                    Color = color,
                    Size = new Godot.Vector2(128, 128),
                    Position = worldPos
                };
                AddChild(transparentSheet);
            }
        }
    }

    private Color GetColor(int alliance)
    {
        switch (alliance)
        {
            case 1:
                return new Color(1, 0, 0, .25f);
            case 2:
                return new Color(0, 0, 1, .25f);
            default:
                return new Color(0, 0, 0, 0);
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