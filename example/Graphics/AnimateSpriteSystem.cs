using System.Collections.Specialized;
using Game;
using TinyEngine.Drawing;
using TinyEngine.Ecs;

namespace Graphics;

public class AnimateSpriteSystem(
    Screen screen,
    SpriteSheet spriteSheet,
    Camera camera,
    TableJoin<SpriteAnimation, WorldPosition> animations) : GameSystem
{
    public override void Execute()
    {
        foreach(var (animation,position) in animations)
        {
            var frame = animation.Animation.Frames[animation.FrameIndex.Index];

            var spriteDimensions = spriteSheet.SpriteAtlas.GetSpriteDimensions(frame.SpriteKey);
            var spriteWS = spriteDimensions.Translate(camera.WorldViewport.TopLeft);
        
            var screenRect = camera.ToScreenSpace(spriteWS);
            if(screen.IsVisible(screenRect))
            {
                spriteSheet.SpriteAtlas.DrawSprite(screen, frame.SpriteKey, screenRect);
            }
        }
    }
}

public class AnimationAdvanceSystem(
    Table<SpriteAnimation> animations,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        foreach(var i in animations.Indices())
        {
            var state = animations[i];
            state.Update(delta.Delta);
            animations.Update(i, state);
        }
    }
}