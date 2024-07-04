using Godot;
using KeepLordWarriors;

public partial class Typeable : Label
{
    public Typeable(string text)
    {
        Text = text;
        Theme = ResourceLoader.Load<Theme>("res://Theme/theme.tres");
        LabelSettings = new LabelSettings()
        {
            FontColor = new Color(0, 0, 0),
            FontSize = 80,
        };
    }
}