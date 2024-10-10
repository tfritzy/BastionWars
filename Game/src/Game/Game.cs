using System.Numerics;
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
    public Time Time;

    private float lastNetworkTick = 0f;
    private float lastWordPlacement = 0f;

    public const float AutoAccrualTime = 3f;
    public const float NetworkTickTime = 1f / 20f;
    public const int InitialWordCount = 5;
    public const float WordPlacementTime = 1f;

    public Game(GameSettings settings)
    {
        Time = new();
        Map = new(this, settings.Map);
        NameKeeps();
        GenerationMode = settings.GenerationMode;
        PlaceInitialWords();
    }

    public void Update(float now)
    {
        Time.Update(now);

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

    private void NetworkTick()
    {
        SendKeepUpdates();
        SendNewProjectileUpdates();
        SendNewSoldierUpdates();
        SendRemovedSoldierUpdates();
        SendNewWordUpdates();
        SendRemovedWordUpdates();
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
            case Oneof_PlayerToGameServer.MsgOneofCase.TypeWord:
                TypeWord word = msg.TypeWord;
                TypeWord(msg.SenderId, word.GridPos);
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

    private void SendKeepUpdates()
    {
        AllKeepUpdates allKeepUpdates = new();
        foreach (Keep keep in Map.Keeps.Values)
        {
            if (keep.SomethingChanged)
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

    private void SendNewProjectileUpdates()
    {
        if (Map.NewProjectiles.Count == 0)
        {
            return;
        }

        NewProjectiles newProjectiles = new();
        foreach (Projectile p in Map.Projectiles)
        {
            if (Map.NewProjectiles.Contains(p.Id))
            {
                newProjectiles.Projectiles.Add(new NewProjectile()
                {
                    Id = p.Id,
                    BirthTime = p.BirthTime,
                    InitialVelocity = p.InitialVelocity.ToSchema(),
                    StartPos = p.StartPos.ToSchema(),
                    TimeWillLand = p.TimeWillLand,
                    GravitationalForce = Constants.ArrowGravity
                });
            }
        }

        AddMessageToOutbox(new Oneof_GameServerToPlayer { NewProjectiles = newProjectiles });
        Map.NewProjectiles.Clear();
    }

    private void SendNewSoldierUpdates()
    {
        if (Map.NewSoldiers.Count == 0)
        {
            return;
        }

        NewSoldiers newSoldiers = new();
        foreach (Soldier s in Map.Soldiers.Values)
        {
            if (Map.NewSoldiers.Contains(s.Id))
            {
                newSoldiers.Soldiers.Add(new NewSoldier()
                {
                    Id = s.Id,
                    MovementSpeed = s.MovementSpeed,
                    Type = s.Type,
                    SourceKeepId = s.SourceKeepId,
                    TargetKeepId = s.TargetKeepId,
                    RowOffset = s.RowOffset
                });
            }
        }

        AddMessageToOutbox(new Oneof_GameServerToPlayer { NewSoldiers = newSoldiers });
        Map.NewSoldiers.Clear();
    }

    private void SendRemovedSoldierUpdates()
    {
        if (Map.RemovedSoldiers.Count == 0)
        {
            return;
        }

        RemovedSoldiers removedSoldiers = new();
        removedSoldiers.SoldierIds.AddRange(Map.RemovedSoldiers);

        AddMessageToOutbox(new Oneof_GameServerToPlayer { RemovedSoldiers = removedSoldiers });
        Map.RemovedSoldiers.Clear();
    }

    private void SendNewWordUpdates()
    {
        if (Map.NewWords.Count == 0)
        {
            return;
        }

        NewWords newWords = new();
        foreach (Vector2Int gridPos in Map.NewWords)
        {
            if (Map.Words.TryGetValue(gridPos, out Word? word))
            {
                if (word == null)
                    continue;

                var owningKeepId = Map.KeepLands[gridPos];
                var owningKeepPos = Map.Grid.GetEntitySchemaPosition(owningKeepId);
                newWords.Words.Add(new NewWord()
                {
                    GridPos = gridPos.ToSchema(),
                    Text = word!.Text,
                    OwningKeepPos = owningKeepPos
                });
            }
        }

        AddMessageToOutbox(new Oneof_GameServerToPlayer { NewWords = newWords });
        Map.NewWords.Clear();
    }

    private void SendRemovedWordUpdates()
    {
        if (Map.RemovedWords.Count == 0)
        {
            return;
        }

        RemovedWords removedWords = new();
        removedWords.Positions.AddRange(Map.RemovedWords);

        AddMessageToOutbox(new Oneof_GameServerToPlayer { RemovedWords = removedWords });
        Map.RemovedWords.Clear();
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

    public void TypeWord(string playerId, V2Int schemaPos)
    {
        Vector2Int pos = Vector2Int.From(schemaPos);
        Logger.Log("Type word at pos: " + pos);
        if (Map.Words.TryGetValue(pos, out Word? word))
        {
            if (word != null)
            {
                Map.RemoveWord(pos);
                uint owningKeep = Map.KeepLands[pos];
                Keep keep = Map.Keeps[owningKeep];
                keep.IncrementSoldierCount(keep.SoldierType, word.Text.Length);
            }
        }
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
            Pos = Map.Grid.GetEntityPosition(k.Id).ToSchema(),
            WarriorCount = k.GetCount(SoldierType.Warrior),
            ArcherCount = k.GetCount(SoldierType.Archer),
        }));
        for (int i = 0; i < state.Keeps.Count; i++)
        {
            state.Keeps[i].Paths.AddRange(GetPathsToOtherKeeps(Map, state.Keeps[i].Id));
        }
        foreach (Vector2Int pos in Map.Words.Keys)
        {
            if (Map.Words.TryGetValue(pos, out Word? word) && word != null)
            {
                var owningKeepId = Map.KeepLands[pos];
                var owningKeepPos = Map.Grid.GetEntitySchemaPosition(owningKeepId);
                state.Words.Add(new NewWord()
                {
                    GridPos = pos.ToSchema(),
                    Text = word.Text,
                    OwningKeepPos = owningKeepPos
                });
            }
        }
        return state;
    }

    private static List<PathToKeep> GetPathsToOtherKeeps(Map map, uint source)
    {
        List<PathToKeep> paths = [];
        foreach (uint kid in map.Keeps.Keys)
        {
            if (kid == source)
                continue;

            var pathMessage = new PathToKeep()
            {
                TargetId = kid,
            };
            var path = map.GetPathBetweenKeeps(source, kid);
            if (path == null) continue;
            pathMessage.Path.AddRange(path.Select(gridP => new V2() { X = gridP.X, Y = gridP.Y }));
            pathMessage.WalkTypes.AddRange(Pathing.GetWalkTypes(path));
            paths.Add(pathMessage);
        }

        return paths;
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

    public void AddMessageToOutbox(Oneof_GameServerToPlayer update, string? recipient = null)
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