using Godot;
using System;

public partial class InteractiveCamera : Camera3D
{
    public override void _Ready()
    {
        var viewport = GetTree().Root;
        viewport.Scaling3DMode = Viewport.Scaling3DModeEnum.Fsr2;
    }
}
