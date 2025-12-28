using Game;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Engine;
using TinyEngine.Input;

public class SpaceShipGameLoop(
    Screen screen,
    InputState inputState, 
    TimeDelta timeDelta, 
    List<GameSystem> schedule) : GameLoop(inputState)
{
    public override bool Tick(TimeSpan delta)
    {
        timeDelta.Delta = delta;
        screen.Renderer.Clear();
        foreach(var system in schedule)
        {
            system.Execute();
        }
        screen.Renderer.Present();

        return true;
    }
}