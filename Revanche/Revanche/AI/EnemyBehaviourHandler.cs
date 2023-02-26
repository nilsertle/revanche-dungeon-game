using Revanche.Core;

namespace Revanche.AI;

public sealed class EnemyBehaviourHandler
{
    private int mPerformeter;
    private int mNextStart = 1;
    private const int Frequency = 5;

    public EnemyBehaviourHandler(LevelState levelState)
    {
        levelState.ArchEnemy?.Initialize(levelState.ArchEnemy);

        foreach (var enemy in levelState.HostileSummons)
        {
            if (enemy.Value is IEnemyBehaviour ai)
            {
                ai.Initialize(enemy.Value);
            }
        }
    }

    public void Update(LevelState levelState, GameLogic gameLogic)
    {
        levelState.ArchEnemy?.UpdateState(levelState, gameLogic);
        foreach (var enemy in levelState.HostileSummons)
        {
            mPerformeter = (mPerformeter + 1) % Frequency;
            if (mPerformeter != 0)
            {
                continue;
            }
            if (enemy.Value is IEnemyBehaviour ai)
            {
                ai.UpdateState(levelState, gameLogic);
            }
        }
        mPerformeter = mNextStart;
        mNextStart = (mNextStart + 1) % Frequency;
    }
}