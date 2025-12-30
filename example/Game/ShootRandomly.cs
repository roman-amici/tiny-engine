using TinyEngine.Ecs;
using TinyEngine.General;

namespace Game;

public struct ShootRandomly
{
    public static readonly TimeSpan ShootCoolDown = TimeSpan.FromSeconds(1); 
    public TimeSpan TimeSinceLastShot {get; set;}
}

public class ShootRandomlySystem(
    Random random,
    TimeDelta delta,
    TableJoin<ShootRandomly, WorldPosition> shooterPositions, 
    Queue<LaserSpawnContext> spawnLaser) : GameSystem
{
    public override void Execute()
    {
        foreach ( var(i,j) in shooterPositions.Indices())
        {
            var shoot = shooterPositions.T1[i];
            shoot.TimeSinceLastShot += delta.Delta;

            if (shoot.TimeSinceLastShot >= ShootRandomly.ShootCoolDown)
            {
                shoot.TimeSinceLastShot = TimeSpan.Zero;
                var rng = random.NextDouble();
                if (rng >= 0.8)
                {
                    var wp = shooterPositions.T2[j];
                    var position = wp.Bounds.BottomLeft;
                    position = position + new Vector2D(wp.Bounds.Width / 2, 1.0);
                    spawnLaser.Enqueue(new(LaserType.Round, position, 1.0, new(0, 250))
                    {
                        FromBottom = true
                    });
                }
            }
            shooterPositions.T1.Update(i, shoot);

        }
    }
}