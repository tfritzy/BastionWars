using Godot;
using KeepLordWarriors;

public partial class Typeable : RichTextLabel
{
    public int Progress { get; private set; }
    private string word;

    public Typeable(string text)
    {
        this.word = text;
        Theme = ResourceLoader.Load<Theme>("res://Theme/theme.tres");
        BbcodeEnabled = true;
        FitContent = true;
        AutowrapMode = TextServer.AutowrapMode.Off;
        UpdateProgress(0);
    }

    public override void _Ready()
    {
        // Text = $"[color=red]Fuck[/color]";
    }

    public void UpdateProgress(int progress)
    {
        Text = $"[color=#222222]{word[..progress]}[/color][color=#555555]{word[progress..]}[/color]";
    }
}