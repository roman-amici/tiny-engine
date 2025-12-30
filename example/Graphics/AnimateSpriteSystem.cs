using System.Collections.Specialized;
using Game;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Graphics;

public class RenderSpriteSystem(
    Screen screen,
    SpriteSheet spriteSheet,
    Camera camera,
    TableJoin<Sprite<GameSprite>, WorldPosition> sprites) : GameSystem
{
    public override void Execute()
    {
        foreach(var (sprite,position) in sprites)
        {
            var spriteDimensions = spriteSheet.SpriteAtlas.GetSpriteDimensions(sprite.SpriteKey);
            spriteDimensions = spriteDimensions.Scaled(sprite.Scale);
            var spriteWS = spriteDimensions.WithTopLeft(position.Bounds.TopLeft);
        
            var screenRect = camera.ToScreenSpace(spriteWS);
            if(screen.IsVisible(screenRect))
            {
                spriteSheet.SpriteAtlas.DrawSprite(screen, sprite.SpriteKey, screenRect);
            }
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

            var sprite = state.Animation.Frames[state.FrameIndex.Index];
            animations.T2.Update(j, sprite.Sprite);
        }
    }
}