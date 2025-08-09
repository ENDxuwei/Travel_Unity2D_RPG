using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆被晕眩状态
/// </summary>
public class SlimeStunState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// 进入状态，闪烁红色特效，调整计时器
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, .1f);

        stateTimer = enemy.stunDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    /// <summary>
    /// 退出状态，关闭闪烁效果
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelColorChange", 0);
        enemy.stats.MakeInvincible(false);
    }

    /// <summary>
    /// 计时，时间到了转换为静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if(rb.velocity.y < .1f && enemy.IsGroundDetected())
        {
            enemy.fx.Invoke("CancelColorChange", 0);
            enemy.anim.SetTrigger("StunFold");
            enemy.stats.MakeInvincible(true);
        }

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

    }
}