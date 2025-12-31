using TinyEngine.Ecs;

namespace Game;

public class GameEndSystem(
    GameState state,
    Singleton<Player> player,
    Queue<SpawnShipMessage> spawnShip
) : GameSystem
{
    public override void Execute()
    {
        if (player.Singlet == null)
        {
            state.Lives--;

            if (state.Lives >= 0)
            {
                spawnShip.Enqueue(new());
            }

        }
    }
}