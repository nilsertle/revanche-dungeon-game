using Revanche.Core;
using Revanche.GameObjects;

namespace Revanche.AI.HostileSummonBehaviour;

public class NoneBehaviour : IEnemyBehaviour
{
    public void UpdateState(LevelState levelState, GameLogic gameLogic)
    {
        // Do nothing
    }

    public void Initialize(Character enemy)
    {
        // Do nothing
    }
}