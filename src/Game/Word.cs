using System.Numerics;

namespace KeepLordWarriors;

public class Word
{
    public string Text { get; private set; }
    public int TypedIndex { get; private set; }
    public V2Int Position { get; set; }

    public Word(string text, V2Int position)
    {
        Text = text;
        TypedIndex = 0;
        Position = position;
    }

    public void HandleKeystroke(char key)
    {
        if (key == Text[TypedIndex])
        {
            TypedIndex++;
        }
        else
        {
            TypedIndex = 0;
        }
    }
}