using System.Collections;
using UnityEngine;

/// <summary>
/// 弓箭手移动状态
/// </summary>
public class ArcherMoveState : ArcherGroundState
{

    public ArcherMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
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
    /// 移动状态，控制速度和转向
    /// </summary>
    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, enemy.rb.velocity.y);

        //撞墙或者没有路反转
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();

            stateMachine.ChangeState(enemy.idleState);
        }
    }
}