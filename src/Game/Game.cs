using Schema;

namespace KeepLordWarriors;

public class Game
{
    public Map Map { get; private set; }
    public GenerationMode GenerationMode { get; private set; }
    public Queue<Schema.OneofUpdate> outbox { get; private set; } = new();

    private float lastNetworkTick = 0f;
    private float lastWordPlacement = 0f;

    public const float AutoAccrualTime = 1f;
    public const float NetworkTickTime = 1f / 20f;
    public const int InitialWordCount = 5;
    public const float WordPlacementTime = 1f;

    public Game(GameSettings settings)
    {
        Map = new(settings.Map);
        GenerationMode = settings.GenerationMode;
        PlaceInitialWords();
    }

    public void Update(float deltaTime)
    {
        BastionAutoAccrue(deltaTime);
        Map.Update(deltaTime);
        PlaceWord(deltaTime);

        lastNetworkTick += deltaTime;
        if (lastNetworkTick >= NetworkTickTime)
        {
            NetworkTick();
            lastNetworkTick = 0f;
        }
    }

    private void NetworkTick()
    {
        SendSoldierPositions();
    }

    private void SendSoldierPositions()
    {
        AllSoldierPositions allSoldierPositions = new();
        foreach (Soldier soldier in Map.Soldiers)
        {
            allSoldierPositions.SoldierPositions.Add(new SoldierPosition
            {
                Id = soldier.Id,
                Pos = Map.Grid.GetEntitySchemaPosition(soldier.Id),
            });
        }

        outbox.Enqueue(new OneofUpdate { AllSoldierPositions = allSoldierPositions });
    }

    private Dictionary<ulong, float> bastionProduceCooldowns = new();
    private void BastionAutoAccrue(float deltaTime)
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

    private void PlaceInitialWords()
    {
        if (GenerationMode != GenerationMode.Word)
        {
            return;
        }

        lastWordPlacement = WordPlacementTime;
        for (int i = 0; i < InitialWordCount; i++)
        {
            Map.PlaceWord();
        }
    }

    private void PlaceWord(float deltaTime)
    {
        if (GenerationMode != GenerationMode.Word)
        {
            return;
        }

        lastWordPlacement += deltaTime;
        if (lastWordPlacement >= WordPlacementTime)
        {
            Map.PlaceWord();
            lastWordPlacement = 0f;
        }
    }
}