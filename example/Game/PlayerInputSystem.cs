using TinyEngine.Ecs;
using TinyEngine.Input;

namespace Game;

public class PlayerInputSystem(
    InputState inputState,
    SingletonJoin<Player,Kinematics> playerKinematics,
    SingletonJoin<Player,WorldPosition> playerPosition,
    TimeDelta delta,
    Queue<LaserSpawnContext> shots
    ) : GameSystem
{
    public override void Execute()
    {
        UpdateState();
        Shoot();
        Steer();
    }

    public void UpdateState()
    {
        if (playerKinematics.S.Singlet == null)
        {
            return;
        }
        var player = playerKinematics.S.Singlet;
        
        player.CanShoot.Update(delta.Delta);
    }

    public void Steer()
    {
        var t = playerKinematics.JoinWithEntity;
        if (t == null)
        {
            return;
        }

        var (entityId,_,kinematics) = t.Value;

        if (inputState.KeysDown.Contains(Key.Left))
        {
            kinematics.Acceleration = new(-200, kinematics.Acceleration.Y);
        }
        else if (inputState.KeysDown.Contains(Key.Right))
        {
            kinematics.Acceleration = new(200, kinematics.Acceleration.Y);
        }
        else
        {
            kinematics.Acceleration = new(0, kinematics.Acceleration.Y);
            kinematics.ApplyFriction(2.0);
        }

        playerKinematics.T.Update(entityId, kinematics);
    }

    public void Shoot()
    {
        if (!inputState.KeysDown.Contains(Key.Space))
        {
            return;
        }

        var t = playerPosition.Join;
        if (t == null)
        {
            return;
        }

        var (player,position) = t.Value;

        if (player.CanShoot.CanShoot)
        {
            var x = position.Bounds.Center.X;
            var y = position.Bounds.TopLeft.Y - 1.0;
            shots.Enqueue(new(LaserType.Flat, new(x, y), 1.0, new(0, -300.0)));
            player.CanShoot.FireShot();
        }
    }
}