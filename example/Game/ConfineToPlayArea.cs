using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class PlayArea(Rect2D area)
{
    public Rect2D Area {get; set;} = area;
}

public struct ConfineToPlayArea{}

public class ConfineToPlayAreaSystem(
    PlayArea area,
    TableJoin<Kinematics, WorldPosition> query) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j) in query.GetIndices())
        {
            var (kinematics,position) = query[i,j];


            if (!area.Area.Contains(position.Bounds.TopLeft) || !area.Area.Contains(position.Bounds.TopRight))
            {
                kinematics.Velocity = new();
                if (position.Bounds.TopLeft.X < 0)
                {
                    position.Bounds = position.Bounds.WithTopLeft(new(0,position.Bounds.TopLeft.Y));
                }
                else
                {
                    position.Bounds = position.Bounds.WithTopRight(new(area.Area.BottomRight.X,position.Bounds.TopLeft.Y));
                }
                query.T1.Update(i, kinematics);
                query.T2.Update(i,position);
            }
        }

    }
}