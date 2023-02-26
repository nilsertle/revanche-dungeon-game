using Revanche.Core;
using Revanche.GameObjects;

namespace Revanche.AI;

public interface IEnemyBehaviour
{    
    public void UpdateState(LevelState levelState, GameLogic gameLogic);
    public void Initialize(Character enemy);
}