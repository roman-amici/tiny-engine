using SDL2;
using TinyEngine.General;

namespace TinyEngine.SdlAbstractions;

public class SdlWindow : IDisposable
{
    public SdlWindow(int width, int height, string title)
    {
        Width = width;
        Height = height;

        WindowPtr = SDL.SDL_CreateWindow(
            title,
            SDL.SDL_WINDOWPOS_UNDEFINED,
            SDL.SDL_WINDOWPOS_UNDEFINED,
            Width,
            Height,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (WindowPtr == IntPtr.Zero)
        {
            var err = SDL.SDL_GetError();
            throw new Exception($"Failed to create window: {err}");
        }
    }

    public int Width {get;}
    public int Height {get;}

    public nint WindowPtr {get;}

    public void Dispose()
    {
        SDL.SDL_DestroyWindow(WindowPtr);
    }
}