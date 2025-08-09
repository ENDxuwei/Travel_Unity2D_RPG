using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家处在地面时的状态,通过该状态进行空中地面转换
/// </summary>
public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
    /// 地面状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        //输入空格跳跃
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);

        //不在地面上时切换空中状态
        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        //左键攻击
        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        //R键释放黑洞
        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackholeUnlocked)
        {
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown!",Color.white);
                return;
            }


            stateMachine.ChangeState(player.blackHole);
        }

        //右键瞄准,需要玩家未分配飞剑
        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
            stateMachine.ChangeState(player.aimSowrd);

        //Q键招架反击
        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttack);

    }

    /// <summary>
    /// 判断玩家是否未分配飞剑，如果没有，则调用回收飞剑
    /// </summary>
    /// <returns></returns>
    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
