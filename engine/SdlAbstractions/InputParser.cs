using SDL2;
using TinyEngine.Input;

namespace TinyEngine.SdlAbstractions;

public class InputParser
{
    public InputParser() {}

    public InputParser(InputState state)
    {
        State = state;
    }

    public InputState State {get;} = new();

    public void ParseInput(SDL.SDL_Event e)
    {
        switch(e.type)
        {
            case SDL.SDL_EventType.SDL_KEYDOWN:
                var keyDown = MapKey(e.key.keysym.sym);
                if (keyDown != null)
                {
                    State.KeysDown.Add(keyDown.Value);
                }
                break;
            case SDL.SDL_EventType.SDL_KEYUP:
                var keyUp = MapKey(e.key.keysym.sym);
                if (keyUp != null)
                {
                    State.KeysDown.Remove(keyUp.Value);
                }
            break;
        }
    }

    public Key? MapKey (SDL.SDL_Keycode code)
    {
        return code switch
        {
            SDL.SDL_Keycode.SDLK_UP => Key.Up,
            SDL.SDL_Keycode.SDLK_DOWN => Key.Down,
            SDL.SDL_Keycode.SDLK_LEFT => Key.Left,
            SDL.SDL_Keycode.SDLK_RIGHT => Key.Right,
            SDL.SDL_Keycode.SDLK_SPACE => Key.Space,
            _ => null
        };

    }
}