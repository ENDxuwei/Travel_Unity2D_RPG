using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆静止状态
/// </summary>
public class SlimeIdleState : SlimeGroundedState
{
    public SlimeIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
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

    }

    /// <summary>
    /// 静止状态，如果计时器归零，进入移动状态
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