using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家滑墙状态
/// </summary>
public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
    /// 滑墙状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        //离开墙壁时立刻切换至空中状态
        if (player.IsWallDetected() == false)
            stateMachine.ChangeState(player.airState);

        //滑墙时跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        //输入反方向则取消滑墙,通过静止状态进入空中状态
        if (xInput != 0 && player.facingDir != xInput)
            stateMachine.ChangeState(player.idleState);

        //滑墙时按住上下方向更改滑行速度
        if (yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

    }

}
