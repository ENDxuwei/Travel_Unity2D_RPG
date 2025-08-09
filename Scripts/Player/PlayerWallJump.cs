using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家滑墙跳跃状态,使用跳跃动画
/// </summary>
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，设置玩家向远离墙壁方向的跳跃速度
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        //增加计时,修复反复进入滑墙的Bug
        stateTimer = 1f;
        player.SetVelocity(5 * -player.facingDir, player.jumpForce);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 滑墙跳，计时，时间结束进入空中状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(player.airState);

        //如果检测到地面，进入静止状态
        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
