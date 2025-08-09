using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家移动状态
/// </summary>
public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，播放声音
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(14,null);
    }

    /// <summary>
    /// 退出状态，关闭声音
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(14);
    }

    /// <summary>
    /// 移动状态，设置玩家速度，当输入为0时，回到静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);


        if (xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);
    }
}
