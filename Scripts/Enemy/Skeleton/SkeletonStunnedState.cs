using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骷髅敌人被晕眩状态
/// </summary>
public class SkeletonStunnedState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，播放特效，被击退
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        //调用红色闪烁特效
        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        stateTimer = enemy.stunDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    /// <summary>
    /// 退出状态，关闭特效
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        //取消闪烁
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    /// <summary>
    /// 晕眩状态，计时退出
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
