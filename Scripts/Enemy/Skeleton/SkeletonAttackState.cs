using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骷髅敌人攻击状态
/// </summary>
public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton enemy;


    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    /// <summary>
    /// 攻击状态，速度归零，攻击结束动画触发后转向战斗状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
