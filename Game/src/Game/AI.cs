using Navigation;

namespace KeepLordWarriors;

public static class AI
{
    public static void GetAction(Game game, string playerId)
    {
        List<Keep> ownedKeeps = game.Map.Keeps.Values.Where(k => k.OwnerId == playerId).ToList();

        foreach (Keep s in ownedKeeps)
        {
            if (s.DeploymentOrders.Count > 0)
            {
                continue;
            }

            HashSet<KeepGraph.Node> neighbors = game.Map.Graph[s.Id].Next;
            foreach (KeepGraph.Node n in neighbors)
            {
                Keep source = game.Map.Keeps[s.Id];
                Keep target = game.Map.Keeps[n.KeepId];

                if (source.Alliance == target.Alliance)
                {
                    continue;
                }

                if (source.TotalMeleePower > target.TotalMeleePower * 1.5f)
                {
                    game.AttackKeep(source.Id, target.Id);
                }
            }
        }
    }

    public static void HarvestFields(Game game, string playerId, float deltaTime)
    {
        Player player = game.Players[playerId];

        if (player.AIConfig == null)
            return;

        player.AIConfig.HarvestCooldown -= deltaTime;
        if (player.AIConfig.HarvestCooldown <= 0)
        {
            Field? f = game.Map.Fields.Values.FirstOrDefault(
                f => f.RemainingGrowthTime == 0 &&
                game.Map.GetOwnerOf(f.Position).OwnerId == playerId);

            if (f == null)
            {
                return;
            }

            game.HandleHarvest(f.Text, playerId);

            player.AIConfig.HarvestCooldown = player.AIConfig.BaseHarvestCooldown;
        }
    }
}