using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 殉道者晕眩状态
/// </summary>
public class ShadyStunnedState : EnemyState
{
    private Enemy_Shady enemy;

    public ShadyStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Shady enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// 进入状态，红色闪烁特效
    /// </summary>
    public override void Enter()
    {
        base.Enter();

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

        enemy.fx.Invoke("CancelColorChange", 0);
    }

    /// <summary>
    /// 晕眩状态，计时切换至静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

    }
}