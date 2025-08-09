using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人类，所有敌人共用
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("眩晕状态设置")]
    public float stunDuration = 1;
    public Vector2 stunDirection = new Vector2(10,12);
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    [Header("移动设置")]
    public float moveSpeed =1.5f;
    public float idleTime =2 ;
    public float battleTime = 7;
    private float defaultMoveSpeed;

    [Header("攻击设置")]
    public float agroDistance = 2;
    public float attackDistance =2;
    public float attackCooldown;
    public float minAttackCooldown = 1;
    public float maxAttackCooldown = 2;
    [HideInInspector] public float lastTimeAttacked;

    public EnemyStateMachine stateMachine { get; private set; }
    public EntityFX fx { get; private set; }
    private Player player;
    public string lastAnimBoolName { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();

        fx = GetComponent<EntityFX>();
    }

    /// <summary>
    /// 敌人状态基类
    /// </summary>
    protected override void Update()
    {
        base.Update();


        stateMachine.currentState.Update();

    }

    /// <summary>
    /// 分配最后动画名称，死亡状态时停止动画
    /// </summary>
    /// <param name="_animBoolName"></param>
    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName;

    /// <summary>
    /// 敌人被减速
    /// </summary>
    /// <param name="_slowPercentage"></param>
    /// <param name="_slowDuration"></param>
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    /// <summary>
    /// 敌人恢复正常速度
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    #region 被冻结

    /// <summary>
    /// 速度归零，模拟敌人被冻结
    /// </summary>
    /// <param name="_timeFrozen"></param>
    public virtual void FreezeTime(bool _timeFrozen)
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    /// <summary>
    /// 被冻结持续时间
    /// </summary>
    /// <param name="_duration"></param>
    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration));

    /// <summary>
    /// 协程，使敌人被冻结一定时间
    /// </summary>
    /// <param name="_seconds"></param>
    /// <returns></returns>
    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #endregion

    #region 反击窗口
    /// <summary>
    /// 打开反击窗口
    /// </summary>
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    /// <summary>
    /// 关闭反击窗口
    /// </summary>
    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 判断敌人是否可以被晕眩
    /// </summary>
    /// <returns></returns>
    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 检测动画是否结束
    /// </summary>
    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    /// <summary>
    /// 特殊攻击动画
    /// </summary>
    public virtual void AnimationSepcialAttackTrigger()
    {

    }

    /// <summary>
    /// 检测玩家是否进入敌人寻敌范围
    /// </summary>
    /// <returns></returns>
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsPlayer);

    /// <summary>
    /// 绘制攻击范围检测辅助线
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }
}
