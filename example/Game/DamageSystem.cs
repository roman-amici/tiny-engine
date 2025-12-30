using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Game;

public class DamageSystem(
    World world,
   TableJoin<Damage,WorldPosition> damagers,
   TableJoin<Health,WorldPosition> healths
) : GameSystem
{
    public override void Execute()
    {
        var toDestroy = new List<EntityId>(0);
        foreach(var (damageEntity, damage,damagePosition) in damagers.WithEntityId())
        {
            foreach( var (healthEntity, health, healthPosition) in healths.WithEntityId())
            {
                if (damageEntity == healthEntity)
                {
                    continue;
                }

                if (damagePosition.Bounds.Intersects(healthPosition.Bounds))
                {
                    if(damage.DestroyOnDamage)
                    {
                        toDestroy.Add(damageEntity);
                    }
                    var newHealth = health.TakeDamage(damage.Amount);
                    healths.T1.Update(healthEntity, newHealth);

                    if (newHealth.Current == 0.0)
                    {
                        
                    }
                }
            }
        }

        foreach(var entityId in toDestroy)
        {
            world.RemoveEntity(entityId);
        }
    }
}

public class ExplodeOnDeathSystem(
    World world,
    SpriteSheet spriteSheet,
    Table<SpriteAnimation> animations,
    Table<Sprite<GameSprite>> sprites,
    Table<WorldPosition> positions,
    Table<DestroyOnAnimationEnd> ends,
    TableJoin<Health, WorldPosition> healthPosition

) : GameSystem
{
    public override void Execute()
    {
        var toRemove = new List<EntityId>(0);

        foreach (var (entity, health,position) in healthPosition.WithEntityId())
        {
            if (health.Current <= 0.0)
            {
                SpawnExplosion(position);
                toRemove.Add(entity);
            }
        }

        foreach(var entityId in toRemove)
        {
            world.RemoveEntity(entityId);
        }
    }

    private void SpawnExplosion(WorldPosition position)
    {
        var entityId = world.SpawnEntity();
        var animation = spriteSheet.Animations.Explosion;
        animations.Add(entityId, new(animation));

        var sprite = animation.Frames.First().Sprite;
        sprites.Add(entityId, sprite);

        var dimensions = spriteSheet.SpriteAtlas.GetSpriteDimensions(sprite.SpriteKey);
        dimensions = dimensions.WithTopLeft(position.Bounds.TopLeft);
        dimensions = dimensions.Scaled(sprite.Scale);
        positions.Add(entityId, new(dimensions));
        ends.Add(entityId, new());
    }
}

public struct Score(double baseScore)
{
    public double BaseScore {get;} = baseScore;
}

public struct Damage(double amount)
{
    public double Amount {get;} = amount;
    public bool DestroyOnDamage {get; set;}
}

public struct Health(double max)
{
    public double MaxHealth {get; set;} = max;
    public double Current {get; set;} = max;

    public Health TakeDamage(double damage)
    {
        var health = new Health()
        {
            MaxHealth = MaxHealth,
            Current = Current
        };
        health.Current -= damage;

        return health;
    }
}