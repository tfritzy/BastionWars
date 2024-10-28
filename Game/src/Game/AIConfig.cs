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
        BaseHarvestCooldown = 5f;
        HarvestCooldown = BaseHarvestCooldown;
        TickCooldown = TickRate;
    }

    public void Update(Game game, string playerId, float deltaTime)
    {
        TickCooldown -= deltaTime;

        if (TickCooldown <= 0)
        {
            AI.HarvestFields(game, playerId, TickRate);
            AI.Attack(game, playerId);
            TickCooldown = TickRate;
        }
    }
}