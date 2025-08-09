using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人状态控制
/// </summary>
public class EnemyState
{

    protected EnemyStateMachine stateMachine;
    protected Enemy enemyBase;
    protected Rigidbody2D rb;

    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    /// <summary>
    /// 状态中，计时器计时
    /// </summary>
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 进入状态，播放动画
    /// </summary>
    public virtual void Enter()
    {
        triggerCalled = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true);

    }

    /// <summary>
    /// 退出状态，分配最后动画名称
    /// </summary>
    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    /// <summary>
    /// 动画结束触发器，保证动作不被打断
    /// </summary>
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
