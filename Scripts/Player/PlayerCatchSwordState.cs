using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家接住返回飞剑的状态
/// </summary>
public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，分配飞剑
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;

        player.fx.PlayDustFX();
        player.fx.ScreenShake(player.fx.shakeSwordImpact);

        //接剑时旋转玩家至飞剑方向
        if (player.transform.position.x > sword.position.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1)
            player.Flip();

        //接剑时给玩家一个反方向的速度模拟作用力
        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .1f);
    }

    /// <summary>
    /// 接剑状态，动画触发后切换到静止状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

}
