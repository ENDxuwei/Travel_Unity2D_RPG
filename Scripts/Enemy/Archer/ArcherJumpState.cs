using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弓箭手跳跃状态
/// </summary>
public class ArcherJumpState : EnemyState
{
    private Enemy_Archer enemy;
    public ArcherJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    /// <summary>
    /// 进入状态，赋予后跳速度
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        enemy.rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, enemy.jumpVelocity.y);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 跳跃状态,落地后切换到战斗状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", rb.velocity.y);

        if(rb.velocity.y < 0 && enemy.IsGroundDetected())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
