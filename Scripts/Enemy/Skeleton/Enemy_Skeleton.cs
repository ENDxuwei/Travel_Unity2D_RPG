using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骷髅敌人类
/// </summary>
public class Enemy_Skeleton : Enemy
{
    #region 状态

    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }

    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }
    #endregion

    /// <summary>
    /// 声明状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, stateMachine, "Idle", this);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    /// <summary>
    /// 状态中效果
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.U))
        {
            stateMachine.ChangeState(stunnedState);
        }

    }

    /// <summary>
    /// 判断骷髅敌人是否可以被晕眩，如果可以，切换至晕眩状态
    /// </summary>
    /// <returns></returns>
    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 骷髅敌人死亡
    /// </summary>
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}
