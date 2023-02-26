using Revanche.GameObjects;

namespace Revanche.AI.HostileSummonBehaviour;

public abstract class EnemyBehaviour
{
    protected Character mEnemy;

    public void Initialize(Character enemy)
    {
        mEnemy = enemy;
    }
}