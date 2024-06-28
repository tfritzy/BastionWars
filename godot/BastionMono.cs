using Godot;
using KeepLordWarriors;

public partial class BastionMono : Node
{
    private Label label;
    private Bastion bastion;

    public BastionMono(Bastion bastion)
    {
        this.bastion = bastion;
        Sprite2D spriteNode = new();
        Texture2D texture = (Texture2D)GD.Load("res://Sprites/keep.png");
        spriteNode.Texture = texture;
        var pos = bastion.Map.Grid.GetEntityPosition(bastion.Id);
        spriteNode.Position = new Vector2(pos.X, pos.Y) * Constants.WorldSpaceToScreenSpace;
        spriteNode.Scale = new Vector2(.6f, .6f);
        AddChild(spriteNode);

        label = new()
        {
            Position = new Vector2(0, -150),
            Scale = new Vector2(3, 3),
        };
        spriteNode.AddChild(label);
    }

    public override void _Process(double delta)
    {
        label.Text = $"{bastion.WarriorCount}|{bastion.ArcherCount}";
    }
}