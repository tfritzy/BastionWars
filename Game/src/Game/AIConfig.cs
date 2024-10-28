using Schema;

namespace KeepLordWarriors;

public class AIConfig
{
    public float BaseHarvestCooldown { get; private set; }
    public float HarvestCooldown;
    private float TickCooldown;

    public const float TickRate = .5f;

    public AIConfig()
    {
        BaseHarvestCooldown = 10f;
        HarvestCooldown = BaseHarvestCooldown;
        TickCooldown = TickRate;
    }

    public void Update(Game game, Player player, float deltaTime)
    {
        TickCooldown -= deltaTime;

        if (TickCooldown <= 0)
        {
            AI.HarvestFields(game, player.Id, deltaTime);

            TickCooldown = TickRate;
        }
    }
}