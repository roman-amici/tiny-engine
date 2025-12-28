using Graphics;
using TinyEngine.Ecs;

namespace Game;

public class ShipSpawner(
    World world,
    SpriteSheet spriteSheet,
    Table<WorldPosition> position,
    Table<SpriteAnimation> animations) : SpawningSystem<object?>(world)
{
    public override void Execute()
    {
        SpawnEntity(null);
    }

    protected override void Spawn(EntityId entityId, object? context)
    {
        position.Add(entityId, new (new(0,0)));
        animations.Add(entityId, new(spriteSheet.Animations.ShipCenter));
    }
}