using System.Numerics;
using System.Text;

namespace KeepLordWarriors;

public class Field
{
    public string Text { get; private set; }
    public int TypedIndex { get; private set; }
    public Vector2Int Position { get; set; }
    public float RemainingGrowthTime { get; set; }
    public bool IsComplete => TypedIndex >= Text.Length;
    private Game Game;
    private bool ReportedGrown;

    public const float GROWTH_TIME = 10f;

    public Field(Game game, Vector2Int position)
    {
        Game = game;
        Text = GetRandomWord().ToLower();
        TypedIndex = 0;
        Position = position;
        ReportedGrown = true;
    }

    public void Update()
    {
        RemainingGrowthTime -= Game.Time.deltaTime;
        RemainingGrowthTime = MathF.Max(0, RemainingGrowthTime);

        if (RemainingGrowthTime == 0 && !ReportedGrown)
        {
            Game.Map.NewlyGrownFields.Add(Position);
            ReportedGrown = true;
        }
    }

    public bool HandleKeystroke(char key)
    {
        if (key == Text[TypedIndex])
        {
            TypedIndex++;

            if (IsComplete)
            {
                Game.Map.HarvestedFields.Add(Position.ToSchema());
                Reset();
            }

            return true;
        }

        return false;
    }

    public void TestSetText(string text)
    {
        Text = text;
    }

    public void Reset()
    {
        Text = GetRandomWord().ToLower();
        RemainingGrowthTime = GROWTH_TIME;
        TypedIndex = 0;
        ReportedGrown = false;
    }

    const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    public string GetRandomWord()
    {
        int length = (int)Game.Randy.SeededInRange(3, 5);
        StringBuilder sb = new();
        for (int i = 0; i < length; i++)
        {
            sb.Append(alphabet[Game.Randy.Seeded.Next(0, alphabet.Length)]);
        }

        return sb.ToString();
    }
}