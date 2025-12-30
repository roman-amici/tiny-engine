using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class PlayArea(Rect2D area)
{
    public Rect2D Area {get; set;} = area;
}

public struct ConfineToPlayArea{}
public struct DeleteOnExitPlayArea{}

public class ConfineToPlayAreaSystem(
    PlayArea area,
    TableJoin<ConfineToPlayArea, Kinematics, WorldPosition> query) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j,k) in query.Indices())
        {
            var (_,kinematics,position) = query[i,j,k];

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
                query.T2.Update(j, kinematics);
                query.T3.Update(k ,position);
            }
        }

    }
}

public class DeleteOnExitPlayAreaSystem(
    World world,
    TableJoin<DeleteOnExitPlayArea, WorldPosition> entities,
    PlayArea area) : GameSystem
{
    public override void Execute()
    {
        var toRemove = new List<EntityId>(0);
        foreach (var (entityId, _, worldPosition) in entities.WithEntityId())
        {
            if (!area.Area.Intersects(worldPosition.Bounds))
            {
                toRemove.Add(entityId);
            }
        }

        foreach(var entityId in toRemove)
        {
            world.RemoveEntity(entityId);
        }
    }
}
