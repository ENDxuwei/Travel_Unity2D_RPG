using UnityEngine;

/// <summary>
/// 殉道者死亡状态
/// </summary>
public class ShadyDeadState : EnemyState
{
    private Enemy_Shady enemy;

    public ShadyDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// 死亡状态，触发自毁
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            enemy.SelfDestroy();
    }
}