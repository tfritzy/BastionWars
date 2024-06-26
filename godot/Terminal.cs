using Godot;
using KeepLordWarriors;

public partial class Terminal : LineEdit
{
    private Game game;

    public Terminal(Game game)
    {
        AnchorLeft = .5f;
        AnchorRight = .5f;
        AnchorTop = 0f;
        AnchorBottom = 0f;

        this.game = game;
    }

    private void OnScreenSizeChanged()
    {
        var dims = GetTree().Root.Size;
        SetSize(new Vector2(dims.X, 0));

        OffsetLeft = 0;
        OffsetTop = dims.Y - Size.Y;
    }

    private void TextSubmit(string text)
    {
        GD.Print("Attack! " + text);

        game.Map.AttackBastion(2, 4);
    }

    public override void _Ready()
    {
        OnScreenSizeChanged();
        GetTree().Root.SizeChanged += OnScreenSizeChanged;
        TextSubmitted += TextSubmit;
        GrabFocus();
    }
}