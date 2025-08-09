using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家死亡状态
/// </summary>
public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    /// <summary>
    /// 进入状态，切换到游戏结束界面
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        GameObject.Find("Canvas").GetComponent<UI>().SwitchOnEndScreen();
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// 死亡状态，速度归0
    /// </summary>
    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
    }
}
