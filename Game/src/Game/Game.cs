using System.Xml;
using Schema;

namespace KeepLordWarriors;

public class Game
{
    public Map Map { get; private set; }
    public GenerationMode GenerationMode { get; private set; }
    public List<Oneof_GameServerToPlayer> Outbox { get; private set; } = [];
    public Dictionary<string, Player> Players { get; private set; } = [];
    public List<string> PlayerIds { get; private set; } = [];

    private float lastNetworkTick = 0f;
    private float lastWordPlacement = 0f;

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

    public void Update()
    {
        BastionAutoAccrue();
        Map.Update();
        PlaceWord();

        lastNetworkTick += Time.deltaTime;
        if (lastNetworkTick >= NetworkTickTime)
        {
            NetworkTick();
            lastNetworkTick = 0f;
            ShipMessages();
        }
    }

    public void HandleCommand(Oneof_PlayerToGameServer msg)
    {
        switch (msg.MsgCase)
        {
            case Oneof_PlayerToGameServer.MsgOneofCase.IssueDeploymentOrder:
                var order = msg.IssueDeploymentOrder;
                SoldierType? soldierType = order.HasSoldierType ? order.SoldierType : null;
                float percent = order.HasPercent ? order.Percent : 1f;
                AttackKeep(order.SourceKeep, order.TargetKeep, soldierType, percent);
                break;
            default:
                Logger.Log("Game got invalid message type from player: " + msg.MsgCase);
                break;
        }
    }

    private void ShipMessages()
    {
        foreach (Oneof_GameServerToPlayer update in Outbox)
        {
            Players[update.RecipientId].MessageQueue.Add(update);
        }

        Outbox.Clear();
    }

    private void NetworkTick()
    {
        SendSoldierPositions();
        SendKeepUpdates();
    }

    private void SendSoldierPositions()
    {
        AllSoldierPositions allSoldierPositions = new();
        foreach (Soldier soldier in Map.Soldiers.Values)
        {
            allSoldierPositions.SoldierPositions.Add(new SoldierState
            {
                Id = soldier.Id,
                Pos = Map.Grid.GetEntitySchemaPosition(soldier.Id),
            });
        }

        AddMessageToOutbox(new Oneof_GameServerToPlayer { AllSoldierPositions = allSoldierPositions });
    }

    private void SendKeepUpdates()
    {
        AllKeepUpdates allKeepUpdates = new();
        foreach (Keep keep in Map.Keeps.Values)
        {
            if (keep.OccupancyChanged)
            {
                allKeepUpdates.KeepUpdates.Add(new KeepUpdate
                {
                    Id = keep.Id,
                    Alliance = keep.Alliance,
                    ArcherCount = keep.ArcherCount,
                    WarriorCount = keep.WarriorCount
                });
                keep.AckOccupancyChanged();
            }
        }

        if (allKeepUpdates.KeepUpdates.Count > 0)
            AddMessageToOutbox(new Oneof_GameServerToPlayer { KeepUpdates = allKeepUpdates });
    }


    private Dictionary<uint, double> bastionProduceCooldowns = new();
    private void BastionAutoAccrue()
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

            bastionProduceCooldowns[bastion.Id] -= Time.deltaTime;

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

    private void PlaceWord()
    {
        if (GenerationMode != GenerationMode.Word)
        {
            return;
        }

        lastWordPlacement += Time.deltaTime;
        if (lastWordPlacement >= WordPlacementTime)
        {
            Map.PlaceWord();
            lastWordPlacement = 0f;
        }
    }

    public void AttackKeep(uint source, uint target, SoldierType? type = null, float percent = 1f)
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
        PlayerIds.Add(player.Id);
        AddMessageToOutbox(new Oneof_GameServerToPlayer { InitialState = GetInitialState() }, player.Id);
    }

    public void DisconnectPlayer(string playerId)
    {
        Players.Remove(playerId);
        PlayerIds.Remove(playerId);
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

            uint landOwner = Map.KeepLands[word.Position];
            if (Map.Keeps[landOwner].Alliance != alliance)
            {
                continue;
            }

            word.HandleKeystroke(key);

            if (word.TypedIndex == word.Text.Length)
            {
                Map.Words[word.Position] = null;
                if (Map.KeepLands.TryGetValue(word.Position, out uint ownerId))
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
        state.Tiles.AddRange(GridToList<TileType>(Map.Tiles));
        state.RenderTiles.AddRange(GridToList<RenderTile>(Map.RenderTiles));
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

    private static List<T> GridToList<T>(T[,] grid)
    {
        List<T> list = new();

        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                list.Add(grid[x, y]);
            }
        }

        return list;
    }

    private void AddMessageToOutbox(Oneof_GameServerToPlayer update, string? recipient = null)
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