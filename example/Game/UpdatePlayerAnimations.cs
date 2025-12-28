using Graphics;
using TinyEngine.Ecs;

namespace Game;

public class UpdatePlayerAnimations(
    SingletonJoin<Player,Kinematics, SpriteAnimation> query,
    SpriteSheet spriteSheet)
    : GameSystem
{

    public override void Execute()
    {
        var t = query.JoinWithEntity;
        if (t == null )
        {
            return;
        }

        var (entity,_,kinematics, animation) = t.Value;

        if (kinematics.Velocity.X < -150.0)
        {
            animation.ChangeAnimation(spriteSheet.Animations.ShipBankLeft);
        }
        else if (kinematics.Velocity.X < 0.0)
        {
            animation.ChangeAnimation(spriteSheet.Animations.ShipLeft);
        }
        else if (kinematics.Velocity.X > 150.0)
        {
            animation.ChangeAnimation(spriteSheet.Animations.ShipBankRight);
        }
        else if (kinematics.Velocity.X > 0.0)
        {
            animation.ChangeAnimation(spriteSheet.Animations.ShipRight);
        }
        else
        {
            animation.ChangeAnimation(spriteSheet.Animations.ShipCenter);
            query.T2.Update(entity, animation);
        }

        query.T2.Update(entity, animation);
    }
}