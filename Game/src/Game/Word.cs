using System.Numerics;
using System.Text;

namespace KeepLordWarriors;

public class Field
{
    public string Text { get; private set; }
    public int TypedIndex { get; private set; }
    public Vector2Int Position { get; set; }
    public float RemainingGrowthTime { get; set; }

    public Field(Vector2Int position)
    {
        Text = GetRandomWord().ToLower();
        TypedIndex = 0;
        Position = position;
    }

    public void Update(float deltaTime)
    {
        RemainingGrowthTime -= deltaTime;
        RemainingGrowthTime = MathF.Max(0, RemainingGrowthTime);
    }

    public void HandleKeystroke(char key)
    {
        if (key == Text[TypedIndex])
        {
            TypedIndex++;
        }
    }

    public void TestSetText(string text)
    {
        Text = text;
    }

    const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    public static string GetRandomWord()
    {
        int length = (int)Randy.ChaoticInRange(3, 5);
        StringBuilder sb = new();
        for (int i = 0; i < length; i++)
        {
            sb.Append(alphabet[Randy.Chaos.Next(0, alphabet.Length)]);
        }

        return sb.ToString();
    }
}