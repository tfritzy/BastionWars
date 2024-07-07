using Godot;

public partial class KeepLabel : Label
{
    private KeepMono bastion;

    public KeepLabel(KeepMono bastion)
    {
        this.bastion = bastion;
        Theme = ResourceLoader.Load<Theme>("res://Theme/theme.tres");
        LabelSettings = new LabelSettings()
        {
            FontColor = new Color(0, 0, 0),
            FontSize = 50,
        };
    }

    public override void _Process(double delta)
    {
        Text = $"{bastion.Keep.WarriorCount}⚔️ {bastion.Keep.ArcherCount}🏹";
    }
}