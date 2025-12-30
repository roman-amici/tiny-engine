using Graphics;
using TinyEngine.Ecs;

namespace Game;

public struct DestroyOnAnimationEnd{}

public class DeleteOnAnimationEndSystem(
    World world,
    TableJoin<DestroyOnAnimationEnd, SpriteAnimation> animations) : GameSystem
{
    public override void Execute()
    {
        var toRemove = new List<EntityId>();
        foreach( var (entityId, _, animation) in animations.WithEntityId())
        {
            if (animation.IsAtEnd())
            {
                toRemove.Add(entityId);
            }
        }

        foreach(var entityId in toRemove)
        {
            world.RemoveEntity(entityId);
        }
    }
}