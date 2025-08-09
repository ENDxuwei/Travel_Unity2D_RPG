using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家冲刺状态,冲刺本质是一个技能,且为超级状态,可以从各种状态进入
/// </summary>
public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入冲刺阶段时，判断能否生成镜像，进入无敌状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        player.skill.dash.CloneOnDash();
        stateTimer = player.dashDuration;

        //冲刺时处于无敌状态
        player.stats.MakeInvincible(true);

    }

    /// <summary>
    /// 退出冲刺状态，判断能否生成镜像，退出无敌
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        player.skill.dash.CloneOnArrival();

        //冲刺结束时设置速度为0,防止滑行
        player.SetVelocity(0, rb.velocity.y);

        player.stats.MakeInvincible(false);
    }

    /// <summary>
    /// 冲刺状态，计时，结束后进入静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        //冲刺撞墙进入滑墙状态
        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        player.fx.CreateAfterImage();
    }
}
