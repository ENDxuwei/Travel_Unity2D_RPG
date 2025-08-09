using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 玩家投掷飞剑时瞄准状态
/// </summary>
public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    /// <summary>
    /// 进入状态，瞄准
    /// </summary>
    public override void Enter()
    {
        base.Enter();

        //激活瞄准辅助线
        player.skill.sword.DotsActive(true);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .2f);
    }

    /// <summary>
    /// 飞剑瞄准状态
    /// </summary>
    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //瞄准时移动鼠标控制玩家转向
        if (player.transform.position.x > mousePosition.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1)
            player.Flip();
    }
}
