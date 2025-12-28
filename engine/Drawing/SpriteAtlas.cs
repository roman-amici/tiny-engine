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

    public void DrawSprite(Screen screen, T sprite, Point2D topLeft, double scale = 1.0)
    {
        if (!SpriteCoordinates.TryGetValue(sprite, out var source))
        {
            source = SpriteCoordinates.FirstOrDefault().Value;
        }

        var destination = new Rect2D(topLeft, new((topLeft.X + source.Width)*scale, (topLeft.Y + source.Height) * scale));

        screen.DrawTexture(SpriteSheet, source, destination);
    }

    public void DrawSprite(Screen screen, T sprite, Rect2D destination)
    {
        if (!SpriteCoordinates.TryGetValue(sprite, out var source))
        {
            source = SpriteCoordinates.FirstOrDefault().Value;
        }

        screen.DrawTexture(SpriteSheet, source, destination);
    }

    public Rect2D GetSpriteDimensions(T sprite)
    {
        var rect = SpriteCoordinates[sprite];

        return new (new(0,0), new(rect.Width,rect.Height));
    }
}