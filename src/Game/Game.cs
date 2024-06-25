namespace KeepLordWarriors;

public class Game
{
    public Map Map { get; private set; }
    public GenerationMode GenerationMode { get; private set; }

    public const float AutoAccrualTime = 1f;

    public Game(GameSettings settings)
    {
        Map = new(settings.Map);
        GenerationMode = settings.GenerationMode;
    }

    public void Update(double deltaTime)
    {
        BastionAutoAccrue(deltaTime);
        Map.Update(deltaTime);
    }

    private Dictionary<ulong, double> bastionProduceCooldowns = new();
    private void BastionAutoAccrue(double deltaTime)
    {
        if (GenerationMode != GenerationMode.AutoAccrue)
        {
            return;
        }

        foreach (Bastion bastion in Map.Bastions)
        {
            if (!bastionProduceCooldowns.ContainsKey(bastion.Id))
            {
                bastionProduceCooldowns[bastion.Id] = AutoAccrualTime;
            }

            bastionProduceCooldowns[bastion.Id] -= deltaTime;

            if (bastionProduceCooldowns[bastion.Id] <= 0)
            {
                bastion.Accrue();
                bastionProduceCooldowns[bastion.Id] = AutoAccrualTime;
            }
        }
    }
}