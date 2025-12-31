using System.Drawing;
using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Game;

public class DamageSystem(
    World world,
   TableJoin<Damage,WorldPosition> damagers,
   TableJoin<Health,WorldPosition> healths,
   Queue<DamagedMessage> damagedMessages
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
                    if (newHealth.Current < health.Current)
                    {
                        damagedMessages.Enqueue(new(healthEntity));
                    }

                    healths.T1.Update(healthEntity, newHealth);
                }
            }
        }

        foreach(var entityId in toDestroy)
        {
            world.RemoveEntity(entityId);
        }
    }
}

public struct DamagedMessage(EntityId damagedEntity)
{
    public EntityId DamagedEntity {get;} = damagedEntity;
}

public struct FlashState
{
    public TimeSpan Duration {get; set;}
    public TimeSpan TimeInState {get; set;}
    public Color FlashColor {get; set;}
}

public class FlashOnDamagedSystem(
    TableJoin<FlashState, Sprite<GameSprite>> flashSprites,
    Queue<DamagedMessage> damagedMessages,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        AddFlashStates();
        UpdateFlashState();
        UpdateSprites();
    }

    private void AddFlashStates()
    {
        while(damagedMessages.TryDequeue(out var damaged))
        {
            var e2 = flashSprites.T2.Find(damaged.DamagedEntity);

            // Entity was already removed
            if (e2 == null)
            {
                continue;
            }

            // Entity is already flashing
            var e1 = flashSprites.T1.Find(damaged.DamagedEntity);
            if (e1 != null)
            {
                continue;
            }

            flashSprites.T1.Add(damaged.DamagedEntity, new()
                    {
                        Duration = TimeSpan.FromSeconds(0.25),
                        FlashColor = Color.YellowGreen,
                    });
        }
    }

    private void UpdateFlashState()
    {
        var toRemove = new List<EntityId>(0);
        for (var i = 0; i < flashSprites.T1.Count; i++)
        {
            var state = flashSprites.T1[i];
            state.TimeInState += delta.Delta;

            if (state.TimeInState > state.Duration)
            {
                toRemove.Add(flashSprites.T1.GetComponent(i).EntityId);
            }

            flashSprites.T1.Update(i, state);
        }

        foreach(var entityId in toRemove)
        {
            flashSprites.T1.Remove(entityId);
        }
    }

    private void UpdateSprites()
    {
        foreach(var (i,j) in flashSprites.Indices())
        {
            var (state,sprite) = flashSprites[i,j];

            sprite.Tint = state.FlashColor;

            flashSprites.T2.Update(j, sprite);
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

        var spriteKey = animation.Frames.First().SpriteKey;
        sprites.Add(entityId, new(spriteKey));

        var dimensions = spriteSheet.GetBounds(spriteKey);
        positions.Add(entityId, new(dimensions.WithTopLeft(position.Bounds.TopLeft)));
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