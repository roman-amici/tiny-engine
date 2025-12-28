namespace Game;

// Player marker for the player entity
public class Player
{
    public CanShootState CanShoot {get;} = new();
}


public class CanShootState
{
    public static TimeSpan CanShootCoolDown = TimeSpan.FromSeconds(0.2); 


    public bool CanShoot {get; private set;} = true;
    public TimeSpan TimeInState {get; private set;}

    public void Update(TimeSpan delta)
    {
        TimeInState += delta;

        if (!CanShoot && TimeInState > CanShootCoolDown)
        {
            CanShoot = true;
            TimeInState = TimeSpan.Zero;
        }
    }
}