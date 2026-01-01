using System.Numerics;
using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class TrajectoryMovementSystem(
    TableJoin<MovementState,WorldPosition> query,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j) in query.Indices())
        {
            var (plan,currentPosition) = query[i,j];
            plan.Update(delta.Delta);

            var center = plan.MovementPlan.GetPosition(plan.Index);

            query.T1.Update(i,plan);
            query.T2.Update(j,new(currentPosition.Bounds.WithCenter(center)));
        }
    }
}

public struct MovementState(MovementPlan plan, TimerIndex index = default)
{
    public MovementPlan MovementPlan {get; } = plan;
    public TimerIndex Index {get; set;} = index;

    public void Update(TimeSpan delta)
    {
        var advance = Index;
        advance.TimeInState += delta;
        
        Index = MovementPlan.NextIndex(advance);
    }
}

public struct Trajectory
{
    public Point2D Start {get; set;}
    public Point2D End {get; set;}
    public TrajectoryTransform Transform {get; set;}
}

public class MovementPlan(SequenceElement<Trajectory>[] trajectories, bool cycle) : TimedSequence<Trajectory>(trajectories, cycle)
{
    public Point2D GetPosition(TimerIndex index)
    {
        var step = Sequence[index.Index];
        var deltaT = step.Duration.TotalSeconds;
        if (deltaT == 0.0)
        {
            return step.Element.Start;
        }

        var deltaX = step.Element.End.X - step.Element.Start.X;
        var deltaY = step.Element.End.Y - step.Element.Start.Y;

        var t = index.TimeInState.TotalSeconds;

        var x = step.Element.Start.X + t * (deltaX / deltaT);
        var y = step.Element.Start.Y + t * (deltaY / deltaT);

        return new(x,y);
    }

    public static MovementPlan Diamond(double radius, Point2D center, TimeSpan legDuration)
    {
        var p1 = new Point2D(center.X + radius, center.Y);
        var p2 = new Point2D(center.X, center.Y - radius);
        var p3 = new Point2D(center.X - radius, center.Y);
        var p4 = new Point2D(center.X, center.Y + radius);

        var plan = new SequenceElement<Trajectory>[4];
        plan[0] = new(new Trajectory()
        {
            Start = p1,
            End = p2,
        }, legDuration);
        plan[1] = new(new Trajectory()
        {
            Start = p2,
            End = p3,
        }, legDuration);
        plan[2] = new(new Trajectory()
        {
            Start = p3,
            End = p4,
        }, legDuration);
        plan[3] = new(new Trajectory()
        {
            Start = p4,
            End = p1,
        }, legDuration);

        return new(plan, true);
    }
}

public enum TrajectoryTransform
{
    Linear,
}