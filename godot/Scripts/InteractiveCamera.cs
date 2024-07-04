using Godot;
using System;

public partial class InteractiveCamera : Camera2D
{
    // Configurable properties
    [Export]
    public float ZoomSpeed = 0.1f; // Speed of zooming in/out
    [Export]
    public float PanSpeed = 10.0f; // Speed of panning


    public InteractiveCamera(Vector2 startPos)
    {
    }

    public override void _Ready()
    {
        AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
        Zoom = new Vector2(.22f, .22f);
        Position += new Vector2(-700, -400);
        var viewport = GetTree().Root;
        viewport.Scaling3DMode = Viewport.Scaling3DModeEnum.Fsr2;
    }

    public override void _Input(InputEvent @event)
    {
        // Handle zoom with mouse wheel
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == MouseButton.WheelDown)
        {
            Zoom *= new Vector2(1.0f - ZoomSpeed, 1.0f - ZoomSpeed);
        }
        else if (@event is InputEventMouseButton eventMouseButtonDown && eventMouseButtonDown.ButtonIndex == MouseButton.WheelUp)
        {
            Zoom *= new Vector2(1.0f + ZoomSpeed, 1.0f + ZoomSpeed);
        }

        if (@event is InputEventMouseMotion eventMouseMotion && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var motion = eventMouseMotion.Relative;
            Position -= motion * Zoom * PanSpeed * 0.1f;
        }
    }
}
