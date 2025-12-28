using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public class UpdateKinematicsSystem(
    TimeDelta delta,
    Table<Kinematics> kinematics, 
    Table<WorldPosition> positions) : GameSystem
{
    public override void Execute()
    {
        for(var i = 0; i < kinematics.Count; i++)
        {
            var comp = kinematics.GetComponent(i);
            var b = comp.Value;

            b.Update(delta.Delta);
            kinematics.Update(i, b);

            var worldPoint = positions.Find(comp.EntityId);
            if (worldPoint != null)
            {
                var wp = worldPoint.Value;
                wp.Update(delta.Delta, b);
                positions.Update(comp.EntityId, wp);
            }
        }
    }
}

public struct Kinematics
{
    // Both expressed in WS point per s
    public Vector2D Acceleration {get; set;}
    public Vector2D Velocity {get; set;}

    public void Update(TimeSpan delta)
    {
        Velocity = Velocity + (delta.TotalSeconds * Acceleration);
    }

    public void ApplyFriction(double nu)
    {
        Acceleration = Acceleration + (-nu * Velocity);
    }
}