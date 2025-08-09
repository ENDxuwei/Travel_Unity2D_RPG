using System.Collections;
using UnityEngine;

/// <summary>
/// 殉道者静止状态
/// </summary>
public class ShadyIdleState : ShadyGroundedState
{
    public ShadyIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    /// <summary>
    /// 进入状态，重置计时器
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
    }

    /// <summary>
    /// 退出状态，播放音效
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 静止状态，计时结束切换至移动状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}