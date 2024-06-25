using Godot;
using KeepLordWarriors;

public partial class Terminal : LineEdit
{
    public Terminal()
    {
        AnchorLeft = .5f;
        AnchorRight = .5f;
        AnchorTop = 0f;
        AnchorBottom = 0f;

        Text = "hello";
    }

    private void OnScreenSizeChanged()
    {
        var dims = GetTree().Root.Size;

        OffsetLeft = dims.X / 2;
        OffsetBottom = 0;
    }

    public override void _Ready()
    {
        OnScreenSizeChanged();
    }
}