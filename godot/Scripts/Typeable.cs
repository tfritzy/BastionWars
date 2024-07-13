using System;
using Godot;
using KeepLordWarriors;

public partial class Typeable : RichTextLabel
{
    public int Progress { get; private set; }
    public Action OnComplete { get; set; }
    private string word;

    public Typeable(string text)
    {
        this.word = text;
        Theme = ResourceLoader.Load<Theme>("res://Theme/theme.tres");
        BbcodeEnabled = true;
        FitContent = true;
        AutowrapMode = TextServer.AutowrapMode.Off;
        ClipContents = false;
    }

    public override void _Ready()
    {
        UpdateProgress(0);
    }

    public void UpdateProgress(int progress)
    {
        Text = $"[outline_size=10][outline_color=#000000][color=#00ff00]{word[..progress]}[/color][color=#ffffff]{word[progress..]}[/color][/outline_color]";
        if (progress >= word.Length)
        {
            OnComplete?.Invoke();
        }
    }

    public void HandleKeystroke(char key)
    {
        if (Progress < word.Length && word[Progress] == key)
        {
            Progress++;
            UpdateProgress(Progress);
        }
        else
        {
            Progress = 0;
            UpdateProgress(Progress);
        }
    }
}