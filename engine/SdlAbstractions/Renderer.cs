using SDL2;

namespace TinyEngine.SdlAbstractions;

public class Renderer : IDisposable
{
    public Renderer(SdlWindow window)
    {
        RendererPtr = SDL.SDL_CreateRenderer(
            window.WindowPtr,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
            SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
        );
        if (RendererPtr == IntPtr.Zero)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to initialize renderer: {err}");
        }
    }

    public nint RendererPtr {get;}

    public Texture LoadTexture(string fileName)
    {
        return Texture.LoadTexture(RendererPtr, fileName);
    }

    public void Clear()
    {
        var result = SDL.SDL_RenderClear(RendererPtr);
        if (result < 0)
        {
            throw new Exception($"Failed to clear render: {SDL.SDL_GetError()}");
        }
    }

    public void Present()
    {
        SDL.SDL_RenderPresent(RendererPtr);
    }

    public void Dispose()
    {
        SDL.SDL_DestroyRenderer(RendererPtr);
    }
}