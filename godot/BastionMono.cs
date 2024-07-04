using Godot;
using KeepLordWarriors;

public partial class BastionMono : Sprite2D
{
    private Label label;
    private Bastion bastion;

    public BastionMono(Bastion bastion)
    {
        this.bastion = bastion;
        Texture2D texture = (Texture2D)GD.Load("res://Sprites/keep.png");
        Texture = texture;

        label = new Label()
        {
            Position = new Vector2(0, -150),
            Scale = new Vector2(3, 3),
        };
        AddChild(label);
    }

    public override void _Process(double delta)
    {
        label.Text = $"{bastion.WarriorCount}|{bastion.ArcherCount}";
    }
}