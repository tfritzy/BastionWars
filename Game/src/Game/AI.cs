using Navigation;

namespace KeepLordWarriors;

public static class AI
{
    public static void Attack(Game game, string playerId)
    {
        List<Keep> ownedKeeps = game.Map.Keeps.Values.Where(k => k.OwnerId == playerId).ToList();

        foreach (Keep s in ownedKeeps)
        {
            if (s.DeploymentOrders.Count > 0)
            {
                continue;
            }

            HashSet<KeepGraph.Node> neighbors = game.Map.Graph[s.Id].Next;
            KeepGraph.Node node = game.Map.Graph[s.Id];
            if (node.DistanceFromFrontline == 0)
            {
                foreach (KeepGraph.Node n in neighbors)
                {
                    Keep source = game.Map.Keeps[s.Id];
                    Keep target = game.Map.Keeps[n.KeepId];

                    if (source.Alliance == target.Alliance)
                    {
                        continue;
                    }

                    if (source.TotalMeleePower > target.TotalMeleePower * 3f)
                    {
                        game.AttackKeep(source.Id, target.Id, percent: .5f);
                        return;
                    }
                }
            }
            else
            {
                KeepGraph.Node closer = neighbors.First(n => n.DistanceFromFrontline == node.DistanceFromFrontline - 1);
                float percentToTransfer = MathF.Max(node.DistanceFromFrontline / 4, 1);
                if (s.TotalCount * percentToTransfer > 10)
                {
                    game.AttackKeep(s.Id, closer.KeepId, percent: percentToTransfer);
                    return;
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
            List<Field> fields = game.Map.Fields.Values.Where(
                f => f.RemainingGrowthTime <= 0 &&
                game.Map.GetOwnerOf(f.Position).OwnerId == playerId)
                .ToList();

            if (fields.Count == 0)
            {
                return;
            }

            Field f = game.Randy.ChaoticElement(fields);

            game.HandleHarvest([f.Text], playerId);

            player.AIConfig.HarvestCooldown = player.AIConfig.BaseHarvestCooldown;
        }
    }
}