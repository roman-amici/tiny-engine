using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class LaserSpawner(
    World world,
    SpriteSheet spriteSheet,
    Table<WorldPosition> position,
    Table<Kinematics> kinematics,
    Table<Damage> damages,
    Table<SpriteAnimation> animations,
    Table<Sprite<GameSprite>> sprites,
    Queue<LaserSpawnContext> messages,
    Table<DeleteOnExitPlayArea> cleanup) : SpawningSystem<LaserSpawnContext>(world)
{
    public override void Execute()
    {
        while(messages.TryDequeue(out var message))
        {
            SpawnEntity(message);
        }
    }

    protected override void Spawn(EntityId entityId, LaserSpawnContext context)
    {
        var laserAnimation = context.LaserType switch
        { 
            LaserType.Flat => spriteSheet.Animations.LaserFlat,
            LaserType.Round => spriteSheet.Animations.LaserRound,
            _ => spriteSheet.Animations.LaserRound
        };
        animations.Add(entityId, new(laserAnimation));

        var sprite = laserAnimation.Frames.First().Sprite;
        sprites.Add(entityId, sprite);

        var dimensions = spriteSheet.SpriteAtlas.GetSpriteDimensions(sprite.SpriteKey);
        dimensions = dimensions.WithBottomLeft(context.Position);
        dimensions = dimensions.Scaled(sprite.Scale);
        dimensions = dimensions.Translated(new(-dimensions.Width / 2,0));

        position.Add(entityId, new(dimensions));

        damages.Add(entityId, new(context.Damage)
        {
            DestroyOnDamage = true
        });
        cleanup.Add(entityId, new());
        kinematics.Add(entityId, new()
        {
            Velocity = context.Velocity,
        });
    }
}

public struct LaserSpawnContext(LaserType laserType, Point2D position, double damage, Vector2D velocity)
{
    public LaserType LaserType {get;} = laserType;
    public Point2D Position {get;} = position;
    public double Damage {get;} = damage;
    public Vector2D Velocity {get;} = velocity;
}

public enum LaserType
{
    Flat,
    Round
}