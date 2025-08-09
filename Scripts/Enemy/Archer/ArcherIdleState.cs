using System.Collections;
using UnityEngine;

/// <summary>
/// 弓箭手静止状态
/// </summary>
public class ArcherIdleState : ArcherGroundState
{
    public ArcherIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
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
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.PlaySFX(24, enemy.transform);
    }

    /// <summary>
    /// 静止状态，计时器归零切换为移动状态
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