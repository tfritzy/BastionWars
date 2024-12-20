using System.Numerics;
using System.Text;
using Navigation;
using Schema;
using SpacialPartitioning;

namespace KeepLordWarriors;

public class Map
{
    public Game Game;
    public TileType[,] Tiles { get; private set; } = new TileType[0, 0];
    public RenderTile[,] RenderTiles { get; private set; } = new RenderTile[0, 0];
    public short[,] Traversable { get; private set; } = new short[0, 0];
    public Grid Grid { get; private set; } = new(0, 0);
    public Dictionary<uint, Keep> Keeps { get; private set; } = [];
    public List<uint> SoldierIds { get; private set; } = [];
    public Dictionary<uint, Soldier> Soldiers { get; private set; } = [];
    public uint[,] KeepLands { get; private set; } = new uint[0, 0];
    public Dictionary<Vector2Int, Field> Fields { get; private set; } = [];
    public List<Projectile> Projectiles { get; private set; } = [];
    public HashSet<uint> NewProjectiles { get; private set; } = [];
    public HashSet<uint> NewSoldiers { get; private set; } = [];
    public HashSet<uint> RemovedSoldiers { get; private set; } = [];
    public HashSet<Vector2Int> NewlyGrownFields { get; private set; } = [];
    public HashSet<V2Int> HarvestedFields { get; private set; } = [];
    public Dictionary<uint, KeepGraph.Node> Graph { get; private set; } = new();
    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);
    private readonly Dictionary<uint, Dictionary<uint, List<Vector2Int>>> keepPaths = [];
    private readonly Dictionary<uint, Dictionary<uint, List<WalkPathType>>> keepWalkPaths = [];

    public Map(Game game, string rawMap)
    {
        Game = game;
        ParseMap(rawMap);
        CalculateKeepPathing();
        CalculateKeepOwnership();
        this.Graph = KeepGraph.Calculate(KeepLands);
        KeepGraph.CalculateDistancesFromFrontline(Keeps, Graph);
        ParseRenderTiles();
    }

    public void Update()
    {
        foreach (Keep keep in Keeps.Values)
        {
            keep.Update();
        }

        for (int i = 0; i < SoldierIds.Count; i++)
        {
            Soldiers[SoldierIds[i]].Update();
        }

        UpdateProjectiles();
        UpdateFields();
    }

    private void UpdateProjectiles()
    {
        for (int i = Projectiles.Count - 1; i >= 0; i--)
        {
            var proj = Projectiles[i];
            if (Game.Time.Now > proj.TimeWillLand)
            {
                uint? hit = Grid.FindCollision(proj.FinalPosition.ToVector2(), .001f, (uint id) =>
                {
                    if (Soldiers.TryGetValue(id, out Soldier? soldier))
                    {
                        if (soldier.Alliance != proj.Alliance)
                        {
                            return true;
                        }
                    }

                    return false;
                });

                if (hit != null)
                {
                    Soldier soldier = Soldiers[hit.Value];
                    soldier.TakeDamage(proj.Damage);
                }

                Projectiles.RemoveAt(i);
            }
        }
    }

    private void UpdateFields()
    {
        foreach (Field f in Fields.Values)
        {
            f.Update();
        }
    }

    public List<Vector2Int>? GetPathBetweenKeeps(uint startId, uint endId)
    {
        if (!keepPaths.ContainsKey(startId) || !keepPaths[startId].ContainsKey(endId))
        {
            return null;
        }

        return keepPaths[startId][endId];
    }

    public List<WalkPathType>? GetWalkPathBetweenKeeps(uint startId, uint endId)
    {
        if (!keepWalkPaths.ContainsKey(startId) || !keepWalkPaths[startId].ContainsKey(endId))
        {
            return null;
        }

        return keepWalkPaths[startId][endId];
    }

    private void CalculateKeepPathing()
    {
        foreach (Keep keep in Keeps.Values)
        {
            ushort[,] pathMap = NavGrid.GetPathMap(Vector2Int.From(Grid.GetEntityPosition(keep.Id)), Traversable);
            Vector2Int sourcePos = Vector2Int.From(Grid.GetEntityPosition(keep.Id));

            foreach (Keep other in Keeps.Values)
            {
                if (keep.Id == other.Id)
                {
                    continue;
                }

                Vector2Int targetPos = Vector2Int.From(Grid.GetEntityPosition(other.Id));
                List<Vector2Int> path = NavGrid.ReconstructPath(sourcePos, targetPos, pathMap);
                if (path == null)
                {
                    continue;
                }

                if (!keepPaths.ContainsKey(keep.Id))
                {
                    keepPaths.Add(keep.Id, new());
                    keepWalkPaths.Add(keep.Id, new());
                }

                keepPaths[keep.Id].Add(other.Id, path);
                keepWalkPaths[keep.Id].Add(other.Id, Pathing.GetWalkTypes(path));
            }
        }
    }

    private void CalculateKeepOwnership()
    {
        Dictionary<uint, Vector2Int> locations = new();
        foreach (Keep keep in Keeps.Values)
        {
            Vector2Int? location = Grid.GetEntityGridPos(keep.Id);
            if (location != null)
            {
                locations.Add(keep.Id, location.Value);
            }
        }

        KeepLands = Ownership.Calculate(Width, Height, locations);
    }

    public void RecalculateFrontlineDistances()
    {
        KeepGraph.CalculateDistancesFromFrontline(Keeps, Graph);
    }

    public void AddSoldier(Soldier soldier, Vector2? pos = null)
    {
        Soldiers.Add(soldier.Id, soldier);
        SoldierIds.Add(soldier.Id);
        Grid.AddEntity(new SpacialPartitioning.Entity(
            pos ?? new Vector2(),
            soldier.Id,
            Soldier.Radius
        ));
        NewSoldiers.Add(soldier.Id);
    }

    public void RemoveSoldier(uint id)
    {
        Soldiers.Remove(id);
        SoldierIds.RemoveAll((sid) => sid == id);
        Grid.RemoveEntity(id);
        RemovedSoldiers.Add(id);
    }

    public void AddProjectile(Projectile projectile)
    {
        Projectiles.Add(projectile);
        NewProjectiles.Add(projectile.Id);
    }

    public float MeleePowerOf(Soldier soldier) => MeleePowerOf(soldier.Type, soldier.Alliance);
    public float MeleePowerOf(SoldierType soldierType, int alliance)
    {
        return soldierType switch
        {
            SoldierType.Archer => 1,
            SoldierType.Warrior => 2,
            _ => 0
        };
    }

    public Keep KeepAt(int index)
    {
        return Keeps.Values.ElementAt(index);
    }

    private void ParseMap(string rawMap)
    {
        rawMap = rawMap.Replace("\r", "");
        rawMap = rawMap.Replace(" ", "");
        string[] terrain = rawMap.Split("\n");

        Tiles = new TileType[terrain[0].Length, terrain.Length];
        Traversable = new short[terrain[0].Length, terrain.Length];
        Grid = new Grid(terrain[0].Length, terrain.Length);
        for (int y = 0; y < terrain.Length; y++)
        {
            for (int x = 0; x < terrain[0].Length; x++)
            {
                switch (terrain[y][x])
                {
                    case 'A':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        var archerKeep = new Keep(
                            Game,
                            SoldierType.Archer,
                            null,
                            alliance: 0);
                        Keeps.Add(archerKeep.Id, archerKeep);
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x + .5f, y + .5f),
                            archerKeep.Id,
                            Keep.Radius
                        ));
                        break;
                    case 'W':
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        var warriorKeep = new Keep(
                            Game,
                            SoldierType.Warrior,
                            null,
                            alliance: 0);
                        Keeps.Add(warriorKeep.Id, warriorKeep);
                        Grid.AddEntity(new SpacialPartitioning.Entity(
                            new Vector2(x + .5f, y + .5f),
                            warriorKeep.Id,
                            Keep.Radius
                        ));
                        break;
                    case 'T':
                        Tiles[x, y] = TileType.Tree;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        break;
                    case 'F':
                        var pos = new Vector2Int(x, y);
                        Tiles[x, y] = TileType.Field;
                        Fields[pos] = new Field(Game, pos);
                        Traversable[x, y] = Navigation.Constants.TRAVERSABLE;
                        break;
                    case 'X':
                        Tiles[x, y] = TileType.Water;
                        Traversable[x, y] = Navigation.Constants.BLOCKED;
                        break;
                    default:
                        Tiles[x, y] = TileType.Land;
                        Traversable[x, y] = Navigation.Constants.TRAVERSABLE;
                        break;
                }
            }
        }
    }

    private void ParseRenderTiles()
    {
        RenderTiles = new RenderTile[Width + 1, Height + 1];

        for (int x = -1; x < Width; x++)
        {
            for (int y = -1; y < Height; y++)
            {
                RenderTiles[x + 1, y + 1] = CalculateRenderTile(x, y);
            }
        }
    }

    public void RecalculateRenderTiles()
    {
        var updatedTiles = new RenderTileUpdates();
        for (int x = -1; x < Width; x++)
        {
            for (int y = -1; y < Height; y++)
            {
                var renderTile = CalculateRenderTile(x, y);
                var existingTile = RenderTiles[x + 1, y + 1];

                if (!AreTilesEqual(renderTile, existingTile))
                {
                    RenderTiles[x + 1, y + 1] = renderTile;
                    updatedTiles.RenderTileUpdates_.Add(new RenderTileUpdate()
                    {
                        Pos = new V2()
                        {
                            X = x + 1,
                            Y = y + 1
                        },
                        RenderTile = renderTile
                    });
                }
            }
        }

        Game.AddMessageToOutbox(new Oneof_GameServerToPlayer()
        {
            RenderTileUpdates = updatedTiles
        });
    }

    public Keep GetOwnerOf(Vector2Int pos)
    {
        return Keeps[KeepLands[pos.X, pos.Y]];
    }

    public uint GetOwnerIdOf(Vector2Int pos)
    {
        return KeepLands[pos.X, pos.Y];
    }

    private RenderTile CalculateRenderTile(int x, int y)
    {
        var renderTile = new RenderTile()
        {
            TileCase = GetRenderTileCase(
                tl: GetMaybeOOBTile(x, y),
                tr: GetMaybeOOBTile(x + 1, y),
                bl: GetMaybeOOBTile(x + 1, y + 1),
                br: GetMaybeOOBTile(x, y + 1)
            ),
        };
        renderTile.CornerAlliance.Add(GetMaybeOOBTileOwnership(x, y));
        renderTile.CornerAlliance.Add(GetMaybeOOBTileOwnership(x + 1, y));
        renderTile.CornerAlliance.Add(GetMaybeOOBTileOwnership(x, y + 1));
        renderTile.CornerAlliance.Add(GetMaybeOOBTileOwnership(x + 1, y + 1));
        if (AllAreOwnedBySameKeep(renderTile.CornerAlliance))
        {
            int owner = renderTile.CornerAlliance[0];
            renderTile.CornerAlliance.Clear();
            renderTile.CornerAlliance.Add(owner);
        }
        renderTile.AllianceCase = GetAllianceCase(renderTile);
        return renderTile;
    }

    private bool AreTilesEqual(RenderTile t1, RenderTile t2)
    {
        if (t1.TileCase != t2.TileCase)
        {
            return false;
        }

        if (t1.AllianceCase != t2.AllianceCase)
        {
            return false;
        }

        if (t1.CornerAlliance.Count != t2.CornerAlliance.Count)
        {
            return false;
        }

        for (int i = 0; i < t1.CornerAlliance.Count; i++)
        {
            if (t1.CornerAlliance[i] != t2.CornerAlliance[i])
            {
                return false;
            }
        }

        return true;
    }

    private RenderAllianceCase GetAllianceCase(RenderTile renderTile)
    {
        Dictionary<int, List<int>> cornerOwnership = parseCornerOwnershipMap(renderTile);

        if (renderTile.TileCase == Constants.TileCase.FULL_LAND)
        {
            if (cornerOwnership.Keys.Count == 1)
            {
                return RenderAllianceCase.FullLandOneOwner;
            }
            else if (cornerOwnership.Keys.Count >= 3)
            {
                return RenderAllianceCase.FullLandIndividualCorners;
            }
            else
            {
                if (cornerOwnership.Values.First().Count == 1 || cornerOwnership.Values.First().Count == 3)
                {
                    return RenderAllianceCase.FullLandSingleRoundedCorner;
                }
                else if (renderTile.CornerAlliance[0] == renderTile.CornerAlliance[3])
                {
                    return RenderAllianceCase.FullLandIndividualCorners;
                }
                else
                {
                    return RenderAllianceCase.FullLandSplitDownMiddle;
                }
            }
        }
        else if (Constants.TileCase.THREE_CORNERS.Contains(renderTile.TileCase))
        {
            switch (cornerOwnership.Keys.Count)
            {
                case 4:
                    return RenderAllianceCase.ThreeCornersThreeOwners;
                case 3:
                    return RenderAllianceCase.ThreeCornersTwoOwners;
                default:
                    return RenderAllianceCase.ThreeCornersOneOwner;
            }
        }
        else if (Constants.TileCase.TWO_LAND_ADJACENT.Contains(renderTile.TileCase))
        {
            switch (cornerOwnership.Keys.Count)
            {
                case 3:
                    return RenderAllianceCase.TwoAdjacentTwoOwners;
                default:
                    return RenderAllianceCase.TwoAdjacentOneOwner;
            }
        }
        else if (Constants.TileCase.TWO_LAND_OPPOSITE.Contains(renderTile.TileCase))
        {
            switch (cornerOwnership.Keys.Count)
            {
                case 3:
                    return RenderAllianceCase.TwoOppositeTwoOwners;
                default:
                    return RenderAllianceCase.TwoOppositeOneOwner;
            }
        }
        else if (Constants.TileCase.SINGLE_LAND_CORNERS.Contains(renderTile.TileCase))
        {
            return RenderAllianceCase.SingleCornerOneOwner;
        }
        else if (renderTile.TileCase == Constants.TileCase.FULL_WATER)
        {
            return RenderAllianceCase.FullWaterNoOnwer;
        }
        else
        {
            throw new Exception("Unknown tile case");
        }
    }

    private static Dictionary<int, List<int>> parseCornerOwnershipMap(RenderTile renderTile)
    {
        Dictionary<int, List<int>> cornerOwnership = new();
        for (int i = 0; i < renderTile.CornerAlliance.Count; i++)
        {
            int alliance = renderTile.CornerAlliance[i];

            if (!cornerOwnership.ContainsKey(alliance))
            {
                cornerOwnership[alliance] = new List<int>();
            }

            cornerOwnership[alliance].Add(i);
        }

        return cornerOwnership;
    }

    private TileType GetMaybeOOBTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return TileType.Water;
        }

        return Tiles[x, y];
    }

    private int GetMaybeOOBTileOwnership(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return -1;
        }

        Vector2Int pos = new Vector2Int(x, y);
        if (Tiles[x, y] == TileType.Water)
        {
            return -1;
        }

        uint keepId = KeepLands[pos.X, pos.Y];
        return Keeps[keepId].Alliance;
    }

    private bool AllAreOwnedBySameKeep(IEnumerable<int> ownership)
    {
        return ownership.All(o => o == ownership.First());
    }

    private uint GetRenderTileCase(
        TileType tl,
        TileType tr,
        TileType bl,
        TileType br
    )
    {
        uint tileCase = 0;
        if (tl != TileType.Water)
        {
            tileCase |= 8;
        }

        if (tr != TileType.Water)
        {
            tileCase |= 4;
        }

        if (br != TileType.Water)
        {
            tileCase |= 2;
        }

        if (bl != TileType.Water)
        {
            tileCase |= 1;
        }

        return tileCase;
    }
}