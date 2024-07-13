using Godot;
using KeepLordWarriors;
using SpacialPartitioning;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class MapMono : GridMap
{
    private Map Map;
    public string MeshLibraryPath = "res://Configs/mesh_set.tres";
    public Dictionary<V2Int, Typeable> Words = new();
    public Dictionary<ulong, KeepMono> KeepMonos = new();
    private Dictionary<V2Int, ColorRect> LandColors = new();

    public MapMono(Map map)
    {
        Map = map;
        Words = new();
        CellSize = new Godot.Vector3(1, 1, 1);
    }

    public override void _Ready()
    {
        MeshLibrary meshLibrary = GD.Load<MeshLibrary>(MeshLibraryPath);
        if (meshLibrary == null)
        {
            GD.Print("Failed to load TileSet from path: " + MeshLibraryPath);
            return;
        }

        this.MeshLibrary = meshLibrary;

        GenerateGrid();
        SpawnKeeps();
        // ColorLands();
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
                SetCellItem(new Vector3I(x, 0, y), (int)type);
            }
        }
    }

    private void SpawnKeeps()
    {
        foreach (var keep in Map.Keeps.Values)
        {
            var keepMono = new KeepMono(keep);
            V2Int keepPos = Map.Grid.GetEntityGridPos(keep.Id).Value;
            keepMono.Position =
                ToGlobal(MapToLocal(new Vector3I(keepPos.X, 0, keepPos.Y)));
            KeepMonos[keep.Id] = keepMono;
            AddChild(keepMono);
            keep.OnCaptured += OnKeepCaptured;
        }
    }

    private void SyncWords()
    {
        foreach (var pos in Map.Words.Keys)
        {
            var word = Map.Words[pos];
            if (word != null && !Words.ContainsKey(pos))
            {
                Camera3D cam = GetViewport().GetCamera3D();
                Words[pos] = new Typeable(word.Text)
                {
                    Position = cam.UnprojectPosition(new Godot.Vector3(pos.X, 0, pos.Y))
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

    // private void ColorLands()
    // {
    //     foreach (var pos in Map.KeepLands.Keys)
    //     {
    //         ulong owner = Map.KeepLands[pos];
    //         if (Map.Keeps.ContainsKey(owner))
    //         {
    //             Keep keep = Map.Keeps[owner];
    //             var worldPos = ToGlobal(MapToLocal(new Vector2I(pos.X, pos.Y)));
    //             var color = GetColor(keep.Alliance);
    //             var transparentSheet = new ColorRect
    //             {
    //                 Color = color,
    //                 Size = new Godot.Vector2(128, 128),
    //                 Position = worldPos
    //             };
    //             AddChild(transparentSheet);
    //             LandColors[pos] = transparentSheet;
    //         }
    //     }
    // }

    private void OnKeepCaptured(ulong keepId)
    {
        foreach (var pos in Map.KeepLands.Keys)
        {
            ulong owner = Map.KeepLands[pos];
            if (owner == keepId)
            {
                var color = GetColor(Map.Keeps[keepId].Alliance);
                LandColors[pos].Color = color;
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

    // private Godot.Vector2 CalculateCenter()
    // {
    //     var rect = GetUsedRect();

    //     GD.Print("Rect: " + rect);

    //     var center_x = Position.X + (rect.Size.X / 2);
    //     var center_y = Position.Y + (rect.Size.Y / 2);

    //     GD.Print("Center: " + center_x + ", " + center_y);

    //     return new Godot.Vector2(center_x, center_y);
    // }
}