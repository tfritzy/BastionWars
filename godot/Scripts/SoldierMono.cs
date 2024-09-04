using Godot;
using KeepLordWarriors;

public partial class SoldierMono : MeshInstance3D
{
    private Game game;
    private uint id;

    public SoldierMono(Game game, uint id)
    {
        this.game = game;
        this.id = id;

        Mesh = new CapsuleMesh();
        Scale = new Vector3(.1f, .1f, .1f);
    }

    public override void _Process(double delta)
    {
        if (!game.Map.Grid.ContainsEntity(id))
        {
            QueueFree();
            return;
        }

        var pos = game.Map.Grid.GetEntityPosition(id);
        Position = new Vector3(pos.X, .8f, pos.Y);
    }
}