using Godot;
using KeepLordWarriors;

public partial class BastionLabel : Label
{
    private BastionMono bastion;

    public BastionLabel(BastionMono bastion)
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
        Text = $"{bastion.Bastion.WarriorCount}⚔️ {bastion.Bastion.ArcherCount}🏹";
    }
}