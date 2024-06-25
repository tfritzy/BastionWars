using Godot;
using KeepLordWarriors;

public partial class Terminal : LineEdit
{
    public Terminal()
    {
        this.AnchorTop = 1;
        this.AnchorBottom = 1;
        this.AnchorLeft = 0;
        this.AnchorRight = 1;

        this.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        Text = "hello";
    }
}