using SDL2;

namespace TinyEngine.SdlAbstractions;

public class Font : IDisposable
{
    public static bool Initialized {get; private set;}

    public static void UseFonts()
    {
        if (Initialized)
        {
            return;
        }

        if (SDL_ttf.TTF_Init() < 0)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to initialize ttf: {err}");
        }
    }

    private Font(nint fontPointer)
    {
        FontPointer = fontPointer;
    }

    public nint FontPointer {get;}

    public static Font LoadFont(string fontPath)
    {
        if (!Initialized)
        {
            throw new InvalidOperationException("Call UseFonts first");
        }

        var fontPtr = SDL_ttf.TTF_OpenFont(fontPath, 24);
        if (fontPtr == IntPtr.Zero)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to load font! : {err}");
        }

        return new Font(fontPtr);
    }

    public TextSurface CreateTextureFromText(nint renderer, string text, int ptSize, SDL.SDL_Color color)
    {
        return TextSurface.RenderText(renderer, this, text, ptSize, color);
    }

    public void Dispose()
    {
        SDL_ttf.TTF_CloseFont(FontPointer);
    }
}