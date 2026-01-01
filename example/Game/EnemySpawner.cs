using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Game;

public struct Enemy{}

public class EnemySpawner(
    World world,
    Queue<EnemySpawnContext> spawnEnemy,
    SpriteSheet spriteSheet,
    Table<WorldPosition> positions,
    Table<Health> healths,
    Table<Damage> damages, // Collision damage
    Table<MovementState> movementPlans,
    Table<SpriteAnimation> animations,
    Table<Sprite<GameSprite>> sprites,
    Table<ShootRandomly> shoot,
    Table<Enemy> enemies,
    Table<Score> scores
    ) : SpawningSystem<EnemySpawnContext>(world)
{
    public override void Execute()
    {
        while (spawnEnemy.TryDequeue(out var context))
        {
            SpawnEntity(context);
        }
    }

    protected override void Spawn(EntityId entityId, EnemySpawnContext context)
    {
        enemies.Add(entityId, new());

        var enemyAnimation = context.EnemyType switch
        { 
            EnemyType.Small => spriteSheet.Animations.EnemySmall,
            EnemyType.Medium => spriteSheet.Animations.EnemyMedium,
            EnemyType.Large => spriteSheet.Animations.EnemyLarge,
            _ => spriteSheet.Animations.EnemySmall
        };
        animations.Add(entityId, new(enemyAnimation));

        var spriteKey = enemyAnimation.Sequence.First().Element;
        sprites.Add(entityId, new(spriteKey));

        var dimensions = spriteSheet.GetBounds(spriteKey);
        positions.Add(entityId, new(dimensions));

        movementPlans.Add(entityId, new(context.Plan));

        shoot.Add(entityId, new());

        AddHealth(entityId, context.EnemyType);
        AddDamage(entityId, context.EnemyType);
        AddScore(entityId, context.EnemyType);
    }

    private void AddDamage(EntityId entityId, EnemyType type)
    {
        var damage = type switch
        {
            EnemyType.Small => 1.0,
            EnemyType.Medium => 2.0,
            EnemyType.Large => 3.0,
            _ => 1.0
        };

        damages.Add(entityId, new(damage));
    }

    private void AddHealth(EntityId entityId, EnemyType type)
    {
        var health = type switch
        {
            EnemyType.Small => 1.0,
            EnemyType.Medium => 2.0,
            EnemyType.Large => 30.0,
            _ => 1.0
        };

        healths.Add(entityId, new(health));
    }

    private void AddScore(EntityId entityId, EnemyType type)
    {
        var score = type switch
        {
          EnemyType.Small => 50,
          EnemyType.Medium => 100,
          EnemyType.Large => 5000,
          _ => 0.0  
        };

        scores.Add(entityId, new(score));
    }
}

public struct EnemySpawnContext(EnemyType enemyType, MovementPlan plan)
{
    public MovementPlan Plan {get;} = plan;
    public EnemyType EnemyType {get;} = enemyType;
}

public enum EnemyType
{
    Small,
    Medium,
    Large
}