using Game;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Engine;

public class SpaceShipGameLoop(Screen screen, TimeDelta timeDelta, List<GameSystem> schedule) : GameLoop
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