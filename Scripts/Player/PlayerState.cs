using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家状态控制
/// </summary>
public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;
    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    /// <summary>
    /// 进入状态，设置动画
    /// </summary>
    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    /// <summary>
    /// 状态进行，检测输入
    /// </summary>
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity", rb.velocity.y);

    }

    /// <summary>
    /// 退出状态，关闭动画
    /// </summary>
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

}
