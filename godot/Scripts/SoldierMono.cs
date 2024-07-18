using Godot;
using KeepLordWarriors;

public partial class SoldierMono : MeshInstance3D
{
    private Game game;
    private ulong id;

    public SoldierMono(Game game, ulong id)
    {
        this.game = game;
        this.id = id;

        Mesh = (Mesh)GD.Load<Mesh>("res://Models/Kenny_TD_Set/detail_treeLarge.obj");
    }

    public override void _Process(double delta)
    {
        if (!game.Map.Grid.ContainsEntity(id))
        {
            QueueFree();
            return;
        }

        var pos = game.Map.Grid.GetEntityPosition(id);
        Position = new Vector3(pos.X, 1, pos.Y);
    }
}