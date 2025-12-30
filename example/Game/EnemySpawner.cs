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
    Table<MovementIndex> movementIndices,
    Table<MovementPlan> movementPlans,
    Table<SpriteAnimation> animations,
    Table<Sprite<GameSprite>> sprites,
    Table<ShootRandomly> shoot,
    Table<Enemy> enemies
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

        var sprite = enemyAnimation.Frames.First().Sprite;
        sprites.Add(entityId, sprite);

        var dimensions = spriteSheet.SpriteAtlas.GetSpriteDimensions(sprite.SpriteKey);
        dimensions = dimensions.Scaled(sprite.Scale);
        positions.Add(entityId, new(dimensions));

        movementPlans.Add(entityId, context.Plan);
        movementIndices.Add(entityId,new());

        shoot.Add(entityId, new());

        AddHealth(entityId, context.EnemyType);
        AddDamage(entityId, context.EnemyType);
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