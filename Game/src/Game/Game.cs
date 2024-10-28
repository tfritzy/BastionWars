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
    public Randy Randy;

    private float lastNetworkTick = 0f;
    private float lastWordPlacement = 0f;

    public const float AutoAccrualTime = 3f;
    public const float NetworkTickTime = 1f / 20f;

    public Game(GameSettings settings)
    {
        Time = new();
        Randy = new(settings.Seed);
        Map = new(this, settings.Map);
        NameKeeps();
        GenerationMode = settings.GenerationMode;
    }

    public void Update(float now)
    {
        Time.Update(now);

        BastionAutoAccrue();
        UpdateAi();
        Map.Update();

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
        SendGrownFieldsUpdate();
        SendHarvestedFieldUpdates();
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
            case Oneof_PlayerToGameServer.MsgOneofCase.HarvestField:
                HarvestField harvest = msg.HarvestField;
                if (harvest.Text.Length == 0) return;
                HandleHarvest(msg.HarvestField.Text, msg.SenderId);
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

    private void SendGrownFieldsUpdate()
    {
        if (Map.NewlyGrownFields.Count == 0)
        {
            return;
        }

        NewGrownFields grownFields = new();
        foreach (Vector2Int gridPos in Map.NewlyGrownFields)
        {
            if (Map.Fields.TryGetValue(gridPos, out Field? field))
            {
                var owningKeepId = Map.GetOwnerIdOf(gridPos);
                var owningKeepPos = Map.Grid.GetEntitySchemaPosition(owningKeepId);
                grownFields.Fields.Add(new GrownField()
                {
                    GridPos = gridPos.ToSchema(),
                    Text = field!.Text,
                });
            }
        }

        AddMessageToOutbox(new Oneof_GameServerToPlayer { NewGrownFields = grownFields });
        Map.NewlyGrownFields.Clear();
    }

    private void SendHarvestedFieldUpdates()
    {
        if (Map.HarvestedFields.Count == 0)
        {
            return;
        }

        HarvestedFields harvested = new();
        harvested.Fields.AddRange(Map.HarvestedFields.Select(pos => new HarvestedField()
        {
            Pos = pos,
            RemainingGrowthTime = Field.GROWTH_TIME,
            TotalGrowthTime = Field.GROWTH_TIME
        }));

        AddMessageToOutbox(new Oneof_GameServerToPlayer { HarvestedFields = harvested });
        Map.HarvestedFields.Clear();
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

    private void UpdateAi()
    {
        foreach (Player p in Players.Values)
        {
            p.AIConfig?.Update(this, p.Id, Time.deltaTime);
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

    public void AttackKeep(uint source, uint target, SoldierType? type = null, float percent = 1f)
    {
        if (!Map.Keeps.ContainsKey(source) || !Map.Keeps.ContainsKey(target))
        {
            return;
        }

        Keep sourceKeep = Map.Keeps[source];
        sourceKeep.SetDeploymentOrder(target, type, percent);
    }

    public bool JoinGame(Player player)
    {
        uint? emptyKeepId = FindEmptyKeep(Map);
        if (emptyKeepId == null)
            return false;

        if (!Players.ContainsKey(player.Id))
        {
            Players[player.Id] = player;
            PlayerIds.Add(player.Id);
            player.Alliance = PlayerIds.Count;
            Map.Keeps[emptyKeepId.Value].OwnerId = player.Id;
            Map.Keeps[emptyKeepId.Value].Capture(player.Alliance, player.Id);
        }

        player.MessageQueue.Add(new Oneof_GameServerToPlayer { InitialState = GetInitialState(player) });
        return true;
    }

    public void DisconnectPlayer(string playerId)
    {
        if (Players.TryGetValue(playerId, out Player? player))
        {
            foreach (Keep k in Map.Keeps.Values)
            {
                if (k.OwnerId == playerId)
                {
                    k.CaptureWithDeferredRecalculate(0, null);
                }

                if (k.Alliance == player.Alliance)
                {
                    k.CaptureWithDeferredRecalculate(0, null);
                }
            }

            foreach (Soldier s in Map.Soldiers.Values)
            {
                if (s.OwnerId == playerId)
                {
                    s.OwnerId = null;
                    s.Alliance = -1;
                }
            }
        }

        Players.Remove(playerId);
        PlayerIds.Remove(playerId);

        Map.RecalculateRenderTiles();
    }

    public void HandleHarvest(string typed, string typer)
    {
        Logger.Log($"{typer} typed '{typed}'");
        if (GenerationMode != GenerationMode.Word)
        {
            return;
        }

        foreach (Field field in Map.Fields.Values)
        {
            uint landOwner = Map.GetOwnerIdOf(field.Position);
            if (Map.Keeps[landOwner].OwnerId != typer)
            {
                continue;
            }

            if (field.HandleTyped(typed))
            {
                uint ownerId = Map.GetOwnerIdOf(field.Position);
                Keep bastion = Map.Keeps[ownerId];
                bastion.IncrementSoldierCount(bastion.SoldierType, field.HarvestValue);
            }
        }
    }

    public InitialState GetInitialState(Player forPlayer)
    {
        var state = new InitialState()
        {
            MapWidth = Map.Width,
            MapHeight = Map.Height,
            OwnAlliance = forPlayer.Alliance,
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
        foreach (Vector2Int pos in Map.Fields.Keys)
        {
            if (Map.Fields.TryGetValue(pos, out Field? field) && field != null)
            {
                if (field.RemainingGrowthTime > 0)
                {
                    continue;
                }

                var owningKeepId = Map.GetOwnerIdOf(pos);
                var owningKeepPos = Map.Grid.GetEntitySchemaPosition(owningKeepId);
                if (Map.Keeps[owningKeepId].OwnerId == forPlayer.Id)
                {
                    state.GrownFields.Add(new GrownField()
                    {
                        GridPos = pos.ToSchema(),
                        Text = field.Text,
                    });
                }
            }
        }
        return state;
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
            pathMessage.Path.AddRange(path.Select(gridP => new V2Int() { X = gridP.X, Y = gridP.Y }));
            pathMessage.WalkTypes.AddRange(Pathing.GetWalkTypes(path));
            paths.Add(pathMessage);
        }

        return paths;
    }

    public static List<T> GridToList<T>(T[,] grid)
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

    public void UpdateFieldVisibilities(uint keepId, string? prevOwnerId, string? newOwnerId)
    {
        List<Field> capturedFields = Map.Fields.Values.Where(f => Map.GetOwnerIdOf(f.Position) == keepId).ToList();

        if (prevOwnerId != null)
        {
            FieldVisibilityChanges msg = new FieldVisibilityChanges();
            msg.NewValues.AddRange(capturedFields.Select(cf => new NewFieldVisibility()
            {
                GridPos = cf.Position.ToSchema(),
                Visible = false,
            }));
            AddMessageToOutbox(
                new Oneof_GameServerToPlayer() { FieldVisibilityChanges = msg },
                prevOwnerId
            );
        }

        if (newOwnerId != null)
        {
            FieldVisibilityChanges msg = new FieldVisibilityChanges();
            msg.NewValues.AddRange(capturedFields.Select(cf => new NewFieldVisibility()
            {
                GridPos = cf.Position.ToSchema(),
                RemainingGrowthTime = cf.RemainingGrowthTime,
                Text = cf.Text,
                TotalGrowthTime = Field.GROWTH_TIME,
                Visible = true,
            }));
            AddMessageToOutbox(
                new Oneof_GameServerToPlayer() { FieldVisibilityChanges = msg },
                newOwnerId
            );
        }
    }

    private static uint? FindEmptyKeep(Map map)
    {
        List<Keep> emptyKeeps = map.Keeps.Values.Where(k => k.OwnerId == null).ToList();
        if (emptyKeeps.Count == 0)
            return null;
        return map.Game.Randy.SeededElement(emptyKeeps).Id;
    }
}