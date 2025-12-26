using System.Drawing;
using SDL2;
using TinyEngine.General;
using TinyEngine.SdlAbstractions;

namespace TinyEngine.Drawing;

public class Screen : IDisposable
{
    public Screen(int width, int height, string title)
    {
        Window = new SdlWindow(width,height,title);
        Renderer = new Renderer(Window);
    }

    public SdlWindow Window {get;}
    public Renderer Renderer {get;}

    // TODO: Support multiple fonts
    public TextCache? Text {get; private set;}

    public void SetBackground(Color color)
    {
        if(SDL.SDL_SetRenderDrawColor(Renderer.RendererPtr, color.R, color.G, color.B, color.A) < 0)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to set draw color: {err}");
        }
    }

    public void DrawTexture(Texture texture, Rect2D source, Rect2D destination)
    {
        var sourceBB = source.ToSdl();
        var destBB = destination.ToSdl();

        var result = SDL.SDL_RenderCopy(Renderer.RendererPtr, texture.TexturePointer, ref sourceBB, ref destBB);

        if (result != 0)
        {
            throw new Exception(SDL.SDL_GetError());
        }
    }

    public void DrawRect(Rect2D rect, Color color)
    {
        SetBackground(color);

        var rectSDL = rect.ToSdl();
        if (SDL.SDL_RenderFillRect(Renderer.RendererPtr, ref rectSDL) < 0)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to draw rect: {err}");
        }
    }

    public void DrawText(TextDefinition draw, Point2D topLeft)
    {
        if (Text == null)
        {
            throw new InvalidOperationException("No font");
        }

        var texture = Text.GetTexture(draw);

        var destinationRect = new Rect2D()
        {
            TopLeft = topLeft,
            BottomRight = new Point2D(topLeft.X + texture.Width, topLeft.Y + texture.Height)
        };

        DrawTexture(texture, destinationRect);
    }

    public void DrawTextCentered(TextDefinition draw, Point2D center)
    {
        if (Text == null)
        {
            throw new InvalidOperationException("No font");
        }

        var texture = Text.GetTexture(draw);

        var left = center.X - (texture.Width / 2);
        var top = center.Y - (texture.Height / 2);

        var destinationRect = new Rect2D()
        {
            TopLeft = new Point2D(left,top),
            BottomRight = new Point2D(left + texture.Width, top + texture.Height)
        };

        DrawTexture(texture, destinationRect);
    }

    private void DrawTexture(Texture texture, Rect2D destinationRect)
    {
        var sourceRect = new Rect2D
        {
            TopLeft = new Point2D(0,0),
            BottomRight = new Point2D(texture.Width, texture.Height)
        };

        DrawTexture(texture, sourceRect, destinationRect);
    }

    public void AddFont(Font font)
    {
        if (Text != null)
        {
            Text.Dispose();
            Text = null;
        }

        Text = new TextCache(Renderer, font);
    }

    public void Dispose()
    {
        Renderer.Dispose();
        Window.Dispose();
        Text?.Dispose();
    }
}