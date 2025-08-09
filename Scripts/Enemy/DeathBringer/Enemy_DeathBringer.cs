using System.Collections;
using UnityEngine;

/// <summary>
/// 死亡使者类
/// </summary>
public class Enemy_DeathBringer : Enemy
{
    public bool bossFightBegun;

    [Header("施法信息")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;

    public float lastTimeCast;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private Vector2 spellOffset;

    [Header("传送信息")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToteleport;
    public float defaultChanceToTeleport = 25;

    #region 状态
    public DeathBringerBattleState battleState { get; private set; }
    public DeathBringerAttackState attackState { get; private set; }
    public DeathBringerIdleState idleState { get; private set; }
    public DeathBringerDeadState deadState { get; private set; }
    public DeathBringerSpellCastState spellCastState { get; private set; }
    public DeathBringerTeleportState teleportState { get; private set; }
    #endregion

    /// <summary>
    /// 实例化所有状态
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        idleState = new DeathBringerIdleState(this, stateMachine, "Idle", this);

        battleState = new DeathBringerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringerAttackState(this, stateMachine, "Attack", this);

        deadState = new DeathBringerDeadState(this, stateMachine, "Idle", this);
        spellCastState = new DeathBringerSpellCastState(this, stateMachine, "SpellCast", this);

        teleportState = new DeathBringerTeleportState(this, stateMachine, "Teleport", this);

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
    /// 死亡
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    /// <summary>
    /// 使用魔法
    /// </summary>
    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * spellOffset.x;

        Vector3 spellPostion = new Vector3(player.transform.position.x + player.facingDir * 2, player.transform.position.y + spellOffset.y);


        GameObject newSpell = Instantiate(spellPrefab, spellPostion, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpell_Controller>().SetupSpell(stats);

    }

    /// <summary>
    /// 寻找传送位置
    /// </summary>
    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroungdBelow().distance + (cd.size.y / 2));

        if (!GroungdBelow() || SomethingIsAround())
        {
            Debug.Log("寻找新的位置");
            FindPosition();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private RaycastHit2D GroungdBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsPlayer);


    /// <summary>
    /// 绘制辅助线
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    /// <summary>
    /// 判断能否传送
    /// </summary>
    /// <returns></returns>
    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToteleport)
        {
            chanceToteleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断能否释放魔法
    /// </summary>
    /// <returns></returns>
    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellStateCooldown)
        {
            return true;
        }
        else
            return false;
    }

}