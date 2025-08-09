using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家静止状态
/// </summary>
public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，设置速度为0
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();

    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (xInput == player.facingDir && player.IsWallDetected())
            return;

        if (xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);
    }
}
