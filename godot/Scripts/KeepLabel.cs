using Godot;

public partial class KeepLabel : Label
{
    private KeepMono bastion;

    public KeepLabel(KeepMono bastion)
    {
        this.bastion = bastion;

        Theme = ResourceLoader.Load<Theme>("res://Theme/theme.tres");
    }

    public override void _Process(double delta)
    {
        Text = $"{bastion.Keep.WarriorCount}⚔️ {bastion.Keep.ArcherCount}🏹";
    }
}