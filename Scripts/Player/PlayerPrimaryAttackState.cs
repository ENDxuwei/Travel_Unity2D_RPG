using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// 玩家普通攻击状态
/// </summary>
public class PlayerPrimaryAttackState : PlayerState
{

    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float comboWindow = 2;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        //重新调用，修复移动后攻击时会出现转向的问题
        xInput = Input.GetAxisRaw("Horizontal");


        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);

        float attackDir = player.facingDir;

        if (xInput != 0)
            attackDir = xInput;

        //向攻击方向进行短距离位移
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        //增加一段协程,防止玩家连续攻击时插入指令
        player.StartCoroutine("BusyFor", .15f);

        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    /// <summary>
    /// 普通攻击状态，当攻击动画结束时触发进入静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        //防止攻击时玩家出现滑动
        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

}
