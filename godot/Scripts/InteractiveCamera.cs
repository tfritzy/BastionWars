using Godot;
using System;

public partial class InteractiveCamera : Camera3D
{
    [Export]
    public float MoveSpeed = 10.0f;
    [Export]
    public float MouseSensitivity = 0.1f;

    public override void _Ready()
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
            RotateX(Mathf.DegToRad(-mouseEvent.Relative.Y * MouseSensitivity));
            RotateY(Mathf.DegToRad(-mouseEvent.Relative.X * MouseSensitivity));
            Vector3 cameraRotation = RotationDegrees;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, -90, 90);
            RotationDegrees = cameraRotation;
        }
    }

    public override void _Process(double delta)
    {
        Vector3 direction = new Vector3();

        if (Input.IsKeyPressed(Key.W))
            direction.Z -= 1;
        if (Input.IsKeyPressed(Key.S))
            direction.Z += 1;
        if (Input.IsKeyPressed(Key.D))
            direction.X += 1;
        if (Input.IsKeyPressed(Key.A))
            direction.X -= 1;
        if (Input.IsKeyPressed(Key.Space))
            direction.Y += 1;
        if (Input.IsKeyPressed(Key.Shift))
            direction.Y -= 1;

        direction = direction.Normalized() * MoveSpeed * (float)delta;
        Translate(direction);
    }
}
