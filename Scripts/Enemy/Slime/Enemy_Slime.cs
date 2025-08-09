using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// 史莱姆类型
/// </summary>
public enum SlimeType
{
    big,
    medium,
    small
}

/// <summary>
/// 史莱姆敌人类
/// </summary>
public class Enemy_Slime : Enemy
{
    [SerializeField] private EnemyStats enemyStats;
    private int levelSlime;

    [Header("史莱姆特殊功能")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int slimesToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;

    #region 状态
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunState stunnedState { get; private set; }
    public SlimeDeathState deadState { get; private set; }
    #endregion

    /// <summary>
    /// 初始化状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        SetupDefaultFacingDir(-1);

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunState(this, stateMachine, "Stunned", this);
        deadState = new SlimeDeathState(this, stateMachine, "Idle", this);
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected override void Start()
    {
        base.Start();

        levelSlime = enemyStats.GetLevel();
        stateMachine.Initialize(idleState);
    }

    /// <summary>
    /// 判断史莱姆是否可以被眩晕
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
    /// 史莱姆死亡,除最小的史莱姆外都会分裂成小一号的史莱姆
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);

        if (slimeType == SlimeType.small)
            return;

        CreateSlimes(slimesToCreate, slimePrefab,levelSlime);

    }

    /// <summary>
    /// 创建史莱姆
    /// </summary>
    /// <param name="_amountOfSlimes"></param>
    /// <param name="_slimePrefab"></param>
    private void CreateSlimes(int _amountOfSlimes, GameObject _slimePrefab,int _level)
    {
        for (int i = 0; i < _amountOfSlimes; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir);
            newSlime.GetComponent<EnemyStats>().level = _level;
        }
    }

    /// <summary>
    /// 初始化分裂史莱姆的方向，位置和组件
    /// </summary>
    /// <param name="_facingDir"></param>
    public void SetupSlime(int _facingDir)
    {
        if (_facingDir != facingDir)
            Flip();

        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * facingDir, yVelocity);

        Invoke("CancelKnockback", 1.5f);

    }

    /// <summary>
    /// 不可以被击退
    /// </summary>
    private void CancelKnockback() => isKnocked = false;
}