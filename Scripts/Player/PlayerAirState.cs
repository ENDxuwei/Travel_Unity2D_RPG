using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家处在空中时的状态,下落
/// </summary>
public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
    /// 玩家空中状态，接触地面后进入到静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        //如果接触到墙体进入滑墙状态
        if (player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        //空中控制移动
        if (xInput != 0)
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);
    }
}
