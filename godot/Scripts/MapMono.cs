using Godot;
using KeepLordWarriors;
using SpacialPartitioning;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class MapMono : GridMap
{
    private Map Map;
    public Dictionary<Vector2Int, Typeable> Words = new();
    public Dictionary<ulong, KeepMono> KeepMonos = new();
    private Dictionary<Vector2Int, MeshInstance3D> lands = new();
    private Dictionary<int, Material> playerMaterials = new();

    public MapMono(Map map)
    {
        Map = map;
        Words = new();
        CellSize = new Godot.Vector3(1, 1, 1);
    }

    public override void _Ready()
    {
        MeshLibrary meshLibrary = GD.Load<MeshLibrary>("res://Configs/mesh_set.tres");
        if (meshLibrary == null)
        {
            GD.Print("Failed to load TileSet from path");
            return;
        }

        this.MeshLibrary = meshLibrary;

        GenerateGrid();
        SpawnKeeps();
        ColorLands();
    }

    public override void _Process(double delta)
    {
        SyncWords();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < Map.RenderTiles.GetLength(0); x++)
        {
            for (int y = 0; y < Map.RenderTiles.GetLength(1); y++)
            {
                RenderTType type = Map.RenderTiles[x, y];
                MeshInstance3D meshInst = new MeshInstance3D();
                meshInst.Mesh = MeshLibrary.GetItemMesh((int)type);
                meshInst.Position = ToGlobal(MapToLocal(new Vector3I(x, 0, y)));
                lands[new Vector2Int(x, y)] = meshInst;
                AddChild(meshInst);
            }
        }
    }

    private void SpawnKeeps()
    {
        foreach (var keep in Map.Keeps.Values)
        {
            var keepMono = new KeepMono(keep);
            Vector2Int keepPos = Map.Grid.GetEntityGridPos(keep.Id).Value;
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

    private void ColorLands()
    {
        foreach (var pos in Map.KeepLands.Keys)
        {
            ulong owner = Map.KeepLands[pos];
            if (Map.Keeps.ContainsKey(owner))
            {
                Keep keep = Map.Keeps[owner];
                var color = GetColor(keep.Alliance);
                SetLandMeshColor(lands[pos], color);
            }
        }
    }

    private void OnKeepCaptured(ulong keepId)
    {
        foreach (var pos in Map.KeepLands.Keys)
        {
            ulong owner = Map.KeepLands[pos];
            if (owner == keepId)
            {
                var color = GetColor(Map.Keeps[keepId].Alliance);
                SetLandMeshColor(lands[pos], color);
            }
        }
    }

    private void SetLandMeshColor(MeshInstance3D mesh, Color color)
    {
        for (int i = 0; i < mesh.Mesh.GetSurfaceCount(); i++)
        {
            var activeMaterial = mesh.Mesh.SurfaceGetMaterial(i);
            var overrideMaterial = activeMaterial.Duplicate() as StandardMaterial3D;
            overrideMaterial.AlbedoColor = color;
            mesh.SetSurfaceOverrideMaterial(i, overrideMaterial);
        }
    }

    private Color GetColor(int alliance)
    {
        switch (alliance)
        {
            case 1:
                return new Color("#7396d5");
            case 2:
                return new Color("#e39764");
            default:
                return new Color("#fcfbf3");
        }
    }
}