using TinyEngine.General;

namespace Game;

public struct WorldPosition(Rect2D position)
{
    public Rect2D Bounds {get; set;} = position;

    public void Update(TimeSpan delta, Kinematics ballistics)
    {
        Bounds = Bounds.Translated(delta.TotalSeconds * ballistics.Velocity);
    }
}