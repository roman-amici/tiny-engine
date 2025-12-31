using Game;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Graphics;

public class TransformSpriteSystem(TableJoin<Sprite<GameSprite>, WorldPosition> sprites) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j) in sprites.Indices())
        {
            var sprite = sprites.T1[i];
            var bounds = sprites.T2[j].Bounds;

            sprite.Transform = bounds;
            sprites.T1.Update(i, sprite);
        }
    }
}

public class RenderSpriteSystem(
    Table<Sprite<GameSprite>> sprites,
    Screen screen,
    SpriteSheet spriteSheet,
    Camera camera) : GameSystem
{
    public override void Execute()
    {
       foreach(var sprite in sprites)
        {
            spriteSheet.SpriteAtlas.DrawSprite(sprite, screen, camera);
        }
    }
}

public class AnimationAdvanceSystem(
    TableJoin<SpriteAnimation,Sprite<GameSprite>> animations,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j) in animations.Indices())
        {
            var state = animations.T1[i];
            state.Update(delta.Delta);
            animations.T1.Update(i, state);

            var spriteKey = state.Animation.Frames[state.FrameIndex.Index].SpriteKey;
            animations.T2.Update(j, new Sprite<GameSprite>(spriteKey));
        }
    }
}