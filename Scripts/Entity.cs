using System.Collections;
using UnityEngine;

/// <summary>
/// 实体基类
/// </summary>
public class Entity : MonoBehaviour
{

    #region 组件
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("击退设置")]
    [SerializeField] protected Vector2 knockbackPower =new Vector2(7,12);
    [SerializeField] protected float knockbackDuration = 0.07f;
    [SerializeField] protected Vector2 knockbackOffset = new Vector2(0.5f,2);
    protected bool isKnocked;

    [Header("碰撞检测")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 1;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = 0.8f;
    [SerializeField] protected LayerMask whatIsGround;

    public int knockbackDir { get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    /// <summary>
    /// Awake基类
    /// </summary>
    protected virtual void Awake()
    {

    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    /// <summary>
    /// Update基类
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// 实体被减速
    /// </summary>
    /// <param name="_slowPercentage"></param>
    /// <param name="_slowDuration"></param>
    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    /// <summary>
    /// 减速结束，返回初始速度
    /// </summary>
    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    #region 被击退

    /// <summary>
    /// 调用协程，实现被击退效果
    /// </summary>
    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    /// <summary>
    /// 协程，确定被击退方向为伤害来源的反方向
    /// </summary>
    /// <param name="_damageDirection"></param>
    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    /// <summary>
    /// 设置击退力度
    /// </summary>
    /// <param name="_knockbackpower"></param>
    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;

    /// <summary>
    /// 协程，受伤时模拟被冲击效果
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);

        //使角色被攻击时不再卡顿
        if(knockbackPower.x > 0 || knockbackPower.y > 0)
            rb.velocity = new Vector2((knockbackPower.x +xOffset)* knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    /// <summary>
    /// 设置冲击力为零，结束被击退
    /// </summary>
    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #endregion

    #region 速度

    /// <summary>
    /// 将速度归零
    /// </summary>
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    /// <summary>
    /// 设置实体速度，速度方向改变时调用翻转函数
    /// </summary>
    /// <param name="_xVelocity"></param>
    /// <param name="_yVelocity"></param>
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region 碰撞
    /// <summary>
    /// 检测是否接触地面
    /// </summary>
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    /// <summary>
    /// 检测是否接触墙体
    /// </summary>
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position , Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    /// <summary>
    /// 绘制碰撞检测辅助线
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x  + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region 翻转
    /// <summary>
    /// 实现翻转
    /// </summary>
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFlipped != null)
            onFlipped();
    }

    /// <summary>
    /// 翻转方向控制
    /// </summary>
    /// <param name="_x"></param>
    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    /// <summary>
    /// 设置默认方向
    /// </summary>
    /// <param name="_direction"></param>
    public virtual void SetupDefaultFacingDir(int _direction)
    {
        facingDir = _direction;

        if(facingDir == -1)
        {
            facingRight = false;
        }
    }
    #endregion


    /// <summary>
    /// 虚类，实体死亡
    /// </summary>
    public virtual void Die()
    {

    }

}
