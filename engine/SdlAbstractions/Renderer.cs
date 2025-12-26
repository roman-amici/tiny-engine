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

    public void Dispose()
    {
        SDL.SDL_DestroyRenderer(RendererPtr);
    }
}