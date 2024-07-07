using Godot;
using KeepLordWarriors;

public partial class KeepMono : Sprite2D
{
    public Keep Keep;

    public KeepMono(Keep keep)
    {
        this.Name = keep.Name;
        this.Keep = keep;
        Texture2D texture = (Texture2D)GD.Load("res://Sprites/keep.png");
        Texture = texture;

        var label = new KeepLabel(this)
        {
            Position = new Vector2(-100, -200)
        };
        AddChild(label);

        var name = new KeepName(this)
        {
            Position = new Vector2(0, 0)
        };
        AddChild(name);
    }
}