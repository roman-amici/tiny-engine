using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Game;

public class ShipSpawner(
    World world,
    SpriteSheet spriteSheet,
    Table<WorldPosition> position,
    Table<SpriteAnimation> animations,
    Table<Kinematics> kinematics,
    Table<ConfineToPlayArea> confine,
    Table<Sprite<GameSprite>> sprites,
    Table<Health> healths,
    PlayArea playArea,
    Singleton<Player> player) : SpawningSystem<object?>(world)
{
    public override void Execute()
    {
        SpawnEntity(null);
    }

    protected override void Spawn(EntityId entityId, object? context)
    {
        var spriteKey = spriteSheet.Animations.ShipRight.Frames[0].SpriteKey;
        var dimensions = spriteSheet.GetBounds(spriteKey);

        position.Add(entityId, new(dimensions.WithBottomLeft(playArea.Area.BottomLeft)));
        animations.Add(entityId, new(spriteSheet.Animations.ShipCenter));
        kinematics.Add(entityId, new());
        sprites.Add(entityId, new());
        confine.Add(entityId, new());
        player.Spawn(entityId, new());
        healths.Add(entityId, new(1.0));
    }
}