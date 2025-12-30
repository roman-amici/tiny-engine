using System.Numerics;
using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class TrajectoryMovementSystem(
    TableJoin<MovementPlan,MovementIndex,WorldPosition> query,
    TimeDelta delta
) : GameSystem
{
    public override void Execute()
    {
        foreach(var (i,j,k) in query.Indices())
        {
            var (plan,index,currentPosition) = query[i,j,k];
            var nextIndex = plan.NextIndex(delta.Delta, index);
            var center = plan.GetPosition(nextIndex);

            query.T2.Update(j,nextIndex);
            query.T3.Update(k,new(currentPosition.Bounds.WithCenter(center)));
        }
    }
}

public struct Trajectory
{
    public Point2D Start {get; set;}
    public Point2D End {get; set;}
    public TrajectoryTransform Transform {get; set;}
    public TimeSpan Duration {get; set;}
}

public struct MovementPlan(Trajectory[] trajectories, bool cycle)
{
    public Trajectory[] Trajectories {get;} = trajectories;
    public bool Cycle {get;} = cycle;
    public bool Complete(MovementIndex index)
    {
        if (!Cycle)
        {
            return false;
        }

        if (index.Index == Trajectories.Length-1 &&
            index.TimeElapsed >= Trajectories.Last().Duration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Point2D GetPosition(MovementIndex index)
    {
        var step = Trajectories[index.Index];
        var deltaT = step.Duration.TotalSeconds;
        if (deltaT == 0.0)
        {
            return step.Start;
        }

        var deltaX = step.End.X - step.Start.X;
        var deltaY = step.End.Y - step.Start.Y;

        var t = index.TimeElapsed.TotalSeconds;

        var x = step.Start.X + t * (deltaX / deltaT);
        var y = step.Start.Y + t * (deltaY / deltaT);

        return new(x,y);
    }

    public MovementIndex NextIndex(TimeSpan delta, MovementIndex currentIndex)
    {
        if(Complete(currentIndex))
        {
            return new()
            {
                Index = currentIndex.Index,
                TimeElapsed = Trajectories.Last().Duration
            };
        }

        var next = new MovementIndex
        {
            Index = currentIndex.Index,
            TimeElapsed = currentIndex.TimeElapsed + delta
        };

        while (true)
        {
            var trajectory = Trajectories[next.Index];
            if (next.TimeElapsed < trajectory.Duration)
            {
                return next;
            }

            next.TimeElapsed -= trajectory.Duration;

            if (Cycle)
            {
                next.Index = (next.Index + 1) % Trajectories.Length;
            }
            else
            {
                next.Index = Math.Min(next.Index + 1, Trajectories.Length-1);
            }
        }
    }

    public static MovementPlan Diamond(double radius, Point2D center, TimeSpan legDuration)
    {
        var p1 = new Point2D(center.X + radius, center.Y);
        var p2 = new Point2D(center.X, center.Y - radius);
        var p3 = new Point2D(center.X - radius, center.Y);
        var p4 = new Point2D(center.X, center.Y + radius);

        var plan = new Trajectory[4];
        plan[0] = new Trajectory()
        {
            Start = p1,
            End = p2,
            Duration = legDuration
        };
        plan[1] = new Trajectory()
        {
            Start = p2,
            End = p3,
            Duration = legDuration
        };
        plan[2] = new Trajectory()
        {
            Start = p3,
            End = p4,
            Duration = legDuration
        };
        plan[3] = new Trajectory()
        {
            Start = p4,
            End = p1,
            Duration = legDuration
        };

        return new(plan, true);
    }
}

public struct MovementIndex()
{
    public TimeSpan TimeElapsed {get; set;} = TimeSpan.Zero;
    public int Index {get; set;}
}

public enum TrajectoryTransform
{
    Linear,
}