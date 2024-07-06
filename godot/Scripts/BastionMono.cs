using Godot;
using KeepLordWarriors;

public partial class BastionMono : Sprite2D
{
    public Keep Bastion;

    public BastionMono(Keep bastion)
    {
        this.Bastion = bastion;
        Texture2D texture = (Texture2D)GD.Load("res://Sprites/keep.png");
        Texture = texture;

        var label = new BastionLabel(this)
        {
            Position = new Vector2(-100, -200)
        };
        AddChild(label);
    }
}