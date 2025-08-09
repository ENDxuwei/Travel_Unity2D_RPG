using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家跳跃状态
/// </summary>
public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，给玩家一个向上的力
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 跳跃状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        //速度为负值时切换成下落
        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);
    }
}
