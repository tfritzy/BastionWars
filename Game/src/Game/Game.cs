using System.Xml;
using Schema;

namespace KeepLordWarriors;

public class Game
{
    public Map Map { get; private set; }
    public GenerationMode GenerationMode { get; private set; }
    public List<OneofUpdate> Outbox { get; private set; } = new();
    public Dictionary<string, Player> Players { get; private set; } = new();

    private double lastNetworkTick = 0f;
    private double lastWordPlacement = 0f;

    public const float AutoAccrualTime = 1f;
    public const float NetworkTickTime = 1f / 20f;
    public const int InitialWordCount = 5;
    public const float WordPlacementTime = 1f;

    public Game(GameSettings settings)
    {
        Map = new(settings.Map);
        NameKeeps();
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
            Packetize();
        }
    }

    private void Packetize()
    {
        foreach (OneofUpdate update in Outbox)
        {
            Players[update.RecipientId].HoldingArea.Add(update);
        }

        foreach (Player player in Players.Values)
        {
            var packets = MessageChunker.Chunk(player.HoldingArea, player.HighestPacketId);
            player.EnqueuePackets(packets);
            player.HoldingArea.Clear();
        }

        Outbox.Clear();
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

        AddMessageToOutbox(new OneofUpdate { AllSoldierPositions = allSoldierPositions });
    }

    private Dictionary<ulong, double> bastionProduceCooldowns = new();
    private void BastionAutoAccrue(double deltaTime)
    {
        if (GenerationMode != GenerationMode.AutoAccrue)
        {
            return;
        }

        foreach (Keep bastion in Map.Keeps.Values)
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

    public void NameKeeps()
    {
        Random random = new();
        HashSet<int> takenNames = new();
        foreach (Keep keep in Map.Keeps.Values)
        {
            int i;
            do
            {
                i = random.Next(0, Constants.KeepNames.Length);
            }
            while (takenNames.Contains(i));

            keep.Name = Constants.KeepNames[i];
            takenNames.Add(i);
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

    public void AttackBastion(ulong source, ulong target, SoldierType? type = null, float percent = 1f)
    {
        if (!Map.Keeps.ContainsKey(source) || !Map.Keeps.ContainsKey(target))
        {
            return;
        }

        Keep sourceKeep = Map.Keeps[source];
        sourceKeep.SetDeploymentOrder(target, type, percent);
    }

    public void JoinGame(Player player)
    {
        Players[player.Id] = player;
        AddMessageToOutbox(new OneofUpdate { InitialState = GetInitialState() }, player.Id);
    }

    public void HandleKeystroke(char key, int alliance)
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

            ulong landOwner = Map.KeepLands[word.Position];
            if (Map.Keeps[landOwner].Alliance != alliance)
            {
                continue;
            }

            word.HandleKeystroke(key);

            if (word.TypedIndex == word.Text.Length)
            {
                Map.Words[word.Position] = null;
                if (Map.KeepLands.TryGetValue(word.Position, out ulong ownerId))
                {
                    Keep? bastion = Map.Keeps[ownerId];
                    bastion?.SetCount(warriors: bastion.GetCount(SoldierType.Warrior) + 1);
                }
            }
        }
    }

    public InitialState GetInitialState()
    {
        var state = new InitialState()
        {
            MapWidth = Map.Width,
            MapHeight = Map.Height,
        };
        state.Tiles.AddRange(Map.Tiles.Cast<TileType>().ToArray());
        state.Keeps.AddRange(Map.Keeps.Values.Select(k => new Schema.KeepState()
        {
            Id = k.Id,
            Name = k.Name,
            Alliance = k.Alliance,
            Pos = Map.Grid.GetEntitySchemaPosition(k.Id),
            WarriorCount = k.GetCount(SoldierType.Warrior),
            ArcherCount = k.GetCount(SoldierType.Archer),
        }));
        return state;
    }

    private void AddMessageToOutbox(OneofUpdate update, string? recipient = null)
    {
        if (recipient == null)
        {
            foreach (Player player in Players.Values)
            {
                var u = update.Clone();
                u.RecipientId = player.Id;
                Outbox.Add(u);
            }
        }
        else
        {
            update.RecipientId = recipient;
            Outbox.Add(update);
        }
    }
}