using System.Drawing;
using TinyEngine.SdlAbstractions;

namespace TinyEngine.Drawing;

public class TextCache(Renderer renderer, Font font) : IDisposable
{
    private Dictionary<TextDefinition,TextSurface> Cache {get;} = new();

    public Texture GetTexture(TextDefinition definition)
    {
        if (!Cache.TryGetValue(definition, out var surface))
        {
            surface = font.CreateTextureFromText(renderer.RendererPtr, definition.Text, definition.FontSize, definition.Color.ToSdl());
            Cache[definition] = surface;
        }

        return surface.Texture;
    }

    public void ClearCache()
    {
        foreach(var texture in Cache.Values)
        {
            texture.Dispose();
        }
        Cache.Clear();
    }

    public void Dispose()
    {
        ClearCache();
    }
}

public struct TextDefinition
{
    public TextDefinition(string text, int fontSize, Color color)
    {
        Text = text;
        FontSize = fontSize;
        Color = color;
    }

    public static TextDefinition Default(string text)
    {
        return new TextDefinition()
        {
            Text = text
        };
    }

    public string Text {get; set;}
    public int FontSize {get; set;} = 18;
    public Color Color {get; set;} = Color.White;

    public override int GetHashCode()
    {
        return (Text,FontSize,(Color.A,Color.R,Color.G,Color.B)).GetHashCode();
    }
}