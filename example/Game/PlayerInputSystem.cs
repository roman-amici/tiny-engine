using Graphics;
using TinyEngine.Drawing;
using TinyEngine.Ecs;
using TinyEngine.Input;

namespace Game;

public class PlayerInputSystem(
    InputState state,
    SingletonJoin<Player,Kinematics> playerKinematics,
    TimeDelta delta
    ) : GameSystem
{
    public override void Execute()
    {
        UpdateState();
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

        if (state.KeysDown.Contains(Key.Left))
        {
            kinematics.Acceleration = new(-200, kinematics.Acceleration.Y);
        }
        else if (state.KeysDown.Contains(Key.Right))
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
}