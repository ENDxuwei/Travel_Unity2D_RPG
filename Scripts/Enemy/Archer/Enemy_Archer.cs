using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 弓箭手敌人
/// </summary>
public class Enemy_Archer : Enemy
{

    [Header("弓箭手特殊信息")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private int arrowDamage;
    public Vector2 jumpVelocity;
    public float jumpCooldown;
    public float safeDistance;
    [HideInInspector] public float lastTimeJumped;

    [Header("额外的碰撞检查")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    [SerializeField] private EnemyStats enemyStats;


    #region 状态

    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherDeadState deadState { get; private set; }
    public ArcherStunnedState stunnedState { get; private set; }
    public ArcherJumpState jumpState { get; private set; }

    #endregion

    /// <summary>
    /// 实例化所有状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        deadState = new ArcherDeadState(this, stateMachine, "Move", this);
        stunnedState = new ArcherStunnedState(this, stateMachine, "Stun", this);
        jumpState = new ArcherJumpState(this, stateMachine, "Jump", this);
    }

    /// <summary>
    /// 初始化组件，默认为静止状态
    /// </summary>
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    /// <summary>
    /// 判断是否可以被眩晕
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
    /// 弓箭手死亡
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 攻击逻辑（射箭）
    /// </summary>
    /// <returns></returns>
    public override void AnimationSepcialAttackTrigger()
    {
        arrowDamage = enemyStats.damage.GetValue() + enemyStats.strength.GetValue();
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);

        newArrow.GetComponent<Arrow_Controller>().SetUpArrow(arrowSpeed * facingDir, stats ,arrowDamage);
    }


    /// <summary>
    /// 身后地面检查
    /// </summary>
    /// <returns></returns>
    public bool GroundBenhundCheck() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);

    /// <summary>
    /// 身后墙壁检查
    /// </summary>
    /// <returns></returns>
    public bool WallBehind() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    /// <summary>
    /// 绘制辅助线
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}