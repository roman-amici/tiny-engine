using TinyEngine.General;
using TinyEngine.SdlAbstractions;

namespace TinyEngine.Drawing;

public class SpriteAtlas<T>(Texture spriteSheet)
where T : notnull
{
    public Texture SpriteSheet { get; } = spriteSheet;

    public Dictionary<T, Rect2D> SpriteCoordinates { get; } = new();

    public void AddSprite(T sprite, uint size, uint x, uint y)
    {
        var topLeft = new Point2D(x * size, y * size);
        var bottomRight = new Point2D(topLeft.X + size, topLeft.Y + size);

        SpriteCoordinates.Add(sprite, new Rect2D(topLeft, bottomRight));
    }

    public void AddSprite(T sprite, int x, int y, uint width, uint height)
    {
        SpriteCoordinates.Add(sprite, new(new(x,y), new(x+width, y+height)));
    }

    public void DrawSprite(Sprite<T> sprite, Screen screen, Camera camera)
    {
        if (!SpriteCoordinates.TryGetValue(sprite.SpriteKey, out var source))
        {
            source = SpriteCoordinates.FirstOrDefault().Value;
        }

        SpriteSheet.SetTextureColor(sprite.Tint);

        var destination = camera.ToScreenSpace(sprite.Transform);
        screen.DrawTexture(SpriteSheet, source, destination, sprite.Rotation);
    }

    public void DrawSprite(RefSprite<T> sprite, Screen screen, Camera camera)
    {
        if (!SpriteCoordinates.TryGetValue(sprite.SpriteKey, out var source))
        {
            source = SpriteCoordinates.FirstOrDefault().Value;
        }

        SpriteSheet.SetTextureColor(sprite.Tint);

        var destination = camera.ToScreenSpace(sprite.Transform);
        screen.DrawTexture(SpriteSheet, source, destination, sprite.Rotation);
    }

    public Rect2D GetSpriteDimensions(T sprite)
    {
        var rect = SpriteCoordinates[sprite];

        return new (new(0,0), new(rect.Width,rect.Height));
    }
}