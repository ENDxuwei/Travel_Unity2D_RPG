using System.Collections;
using UnityEngine;

/// <summary>
/// 暗影殉道者敌人类
/// </summary>
public class Enemy_Shady : Enemy
{
    [Header("暗影殉道者特殊信息")]
    public float battleStateMoveSpeed;
    [SerializeField] private GameObject explosivePrefab;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;


    #region 状态

    public ShadyIdleState idleState { get; private set; }
    public ShadyMoveState moveState { get; private set; }
    public ShadyDeadState deadState { get; private set; }
    public ShadyStunnedState stunnedState { get; private set; }
    public ShadyBattleState battleState { get; private set; }

    #endregion

    /// <summary>
    /// 实例化所有状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        idleState = new ShadyIdleState(this, stateMachine, "Idle", this);
        moveState = new ShadyMoveState(this, stateMachine, "Move", this);
        deadState = new ShadyDeadState(this, stateMachine, "Dead", this);
        stunnedState = new ShadyStunnedState(this, stateMachine, "Stunned", this);
        battleState = new ShadyBattleState(this, stateMachine, "MoveFast", this);

    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    /// <summary>
    /// 检测是否可以被晕眩
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
    /// 暗影殉道者死亡
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 特殊动画触发
    /// </summary>
    public override void AnimationSepcialAttackTrigger()
    {
        GameObject newExplosive = Instantiate(explosivePrefab, transform.position, Quaternion.identity);

        newExplosive.GetComponent<Explosive_Controller>().SetupExplosive(stats, growSpeed, maxSize, attackDistance);

        cd.enabled = false;
        rb.gravityScale = 0;
    }

    /// <summary>
    /// 自我销毁
    /// </summary>
    public void SelfDestroy() => Destroy(gameObject);

}