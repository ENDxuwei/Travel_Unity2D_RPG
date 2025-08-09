using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骷髅敌人静止状态
/// </summary>
public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton enemy) : base(_enemyBase, _stateMachine, _animBoolName, enemy)
    {
    }

    /// <summary>
    /// 进入状态
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

        AudioManager.instance.PlaySFX(24,enemy.transform);
    }

    /// <summary>
    /// 骷髅静止
    /// </summary>
    public override void Update()
    {
        base.Update();

        //敌人刷新或需要重新设定逻辑时，静止一段时间自动转移至行走状态（巡逻）
        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);

    }
}
