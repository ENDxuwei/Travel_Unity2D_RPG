using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 史莱姆移动状态
/// </summary>
public class SlimeMoveState : SlimeGroundedState
{
    public SlimeMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
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
    }

    /// <summary>
    /// 移动状态，赋予速度
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, enemy.rb.velocity.y);

        //  检测是否接近墙体或无地面，控制转向
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
            stateTimer = 0.2f;
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}