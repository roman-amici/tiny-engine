using TinyEngine.General;

namespace Game;

public struct WorldPosition(Point2D topLeft)
{
    public Point2D TopLeft {get; set;} = topLeft;
}