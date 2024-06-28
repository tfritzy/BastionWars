using Godot;
using KeepLordWarriors;

public partial class SoldierMono : Sprite2D
{
    private Game game;
    private ulong id;

    public SoldierMono(Game game, ulong id)
    {
        this.game = game;
        this.id = id;

        Texture = (Texture2D)GD.Load("res://icon.svg");
        Scale = new Vector2(.1f, .1f);
    }

    public override void _Process(double delta)
    {
        if (!game.Map.Grid.ContainsEntity(id))
        {
            QueueFree();
            return;
        }

        var pos = game.Map.Grid.GetEntityPosition(id);
        Position = new Vector2(pos.X, pos.Y) * Constants.WorldSpaceToScreenSpace;
    }
}