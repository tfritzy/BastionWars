using System;
using Godot;
using KeepLordWarriors;

public partial class KeepMono : MeshInstance3D
{
    public Keep Keep;
    public KeepName NameLabel { get; }
    public Action<Keep> OnSelect { get; set; }

    public KeepMono(Keep keep)
    {
        this.Name = keep.Name;
        this.Keep = keep;
        Mesh = (Mesh)GD.Load("res://Models/Kenny_TD_Set/towerRound_sampleF.obj");

        var label = new KeepLabel(this)
        {
            Position = new Vector2(-100, -200)
        };
        AddChild(label);

        NameLabel = new KeepName(this)
        {
            OnComplete = () => OnSelect?.Invoke(keep)
        };
        AddChild(NameLabel);
    }

    public override void _Ready()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        NameLabel.Position = cam.UnprojectPosition(Position);
    }
}