using SDL2;

namespace TinyEngine.Engine;

public abstract class GameLoop
{
    public void Run()
    {
        var running = true;
        var time = DateTime.Now;
        while (running)
        {
            running = PollEvents();
            if (running == false)
            {
                break;
            }

            var now = DateTime.Now;
            var delta = now - time;
            Tick(delta);
            time = now;
        }
    }

    public abstract bool Tick(TimeSpan delta);

    private bool PollEvents()
    {
        while (SDL.SDL_PollEvent(out var e) == 1)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    return false;
            }
        }

        return true;
    }
}