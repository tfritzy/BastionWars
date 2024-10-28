using System.Numerics;
using System.Text;

namespace KeepLordWarriors;

public class Field
{
    public string Text { get; private set; }
    public Vector2Int Position { get; set; }
    public float RemainingGrowthTime { get; set; }
    public int HarvestValue => HARVEST_VALUE;
    private Game Game;
    private bool ReportedGrown;

    public const float GROWTH_TIME = 60f;
    public const int HARVEST_VALUE = 10;

    public Field(Game game, Vector2Int position)
    {
        Game = game;
        Text = GetRandomWord().ToLower();
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

    public bool HandleTyped(string typed)
    {
        if (typed == Text)
        {
            Game.Map.HarvestedFields.Add(Position.ToSchema());
            Reset();
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
        ReportedGrown = false;
    }

    const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    public string GetRandomWord()
    {
        int length = 3;
        StringBuilder sb = new();
        for (int i = 0; i < length; i++)
        {
            sb.Append(alphabet[Game.Randy.Seeded.Next(0, alphabet.Length)]);
        }

        return sb.ToString();
    }
}