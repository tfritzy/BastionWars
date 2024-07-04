using Schema;

namespace KeepLordWarriors;

public class Game
{
    public Map Map { get; private set; }
    public GenerationMode GenerationMode { get; private set; }
    public Queue<Schema.OneofUpdate> outbox { get; private set; } = new();

    private double lastNetworkTick = 0f;
    private double lastWordPlacement = 0f;

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

    public void Update(double deltaTime)
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

    private void PlaceWord(double deltaTime)
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

    public void HandleKeystroke(char key)
    {
        if (GenerationMode != GenerationMode.Word)
        {
            return;
        }

        foreach (Word? word in Map.Words.Values)
        {
            if (word == null)
            {
                continue;
            }

            word.HandleKeystroke(key);

            if (word.TypedIndex == word.Text.Length)
            {
                Map.Words[word.Position] = null;
                if (Map.BastionLands.TryGetValue(word.Position, out ulong ownerId))
                {
                    Bastion? bastion = Map.Bastions.FirstOrDefault(b => b.Id == ownerId);
                    bastion?.SetCount(warriors: bastion.GetCount(SoldierType.Warrior) + 1);
                }
            }
        }
    }
}