using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary>
/// 飞剑技能管理器
/// </summary>
public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;


    private float freezeTimeDuration;
    private float returnSpeed = 12;


    #region 模式
    [Header("穿刺设置")]
    private float pierceAmount;

    [Header("弹射设置")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("旋转设置")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    #endregion

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    /// <summary>
    /// 销毁,防止飞太远无法回收
    /// </summary>
    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    #region 构造函数
    /// <summary>
    /// 构造飞剑技能
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="_gravityScale"></param>
    /// <param name="_player"></param>
    /// <param name="_freezeTimeDuration"></param>
    /// <param name="_returnSpeed"></param>
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);


        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    /// <summary>
    /// 构造弹射方法
    /// </summary>
    /// <param name="_isBouncing"></param>
    /// <param name="_amountOfBounces"></param>
    /// <param name="_bounceSpeed"></param>
    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    #endregion

    private void Update()
    {
        //使飞剑始终面向飞行方向，不然命中目标时会出现方向错误
        if (canRotate)
            transform.right = rb.velocity;


        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        BounceLogic();
        SpinLogic();
    }

    /// <summary>
    /// 召回飞剑
    /// </summary>
    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        transform.parent = null;
        isReturning = true;

    }

    #region 旋转模式
    /// <summary>
    /// 飞剑旋转模式逻辑
    /// </summary>
    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>(),.4f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 停止飞行进入旋转模式
    /// </summary>
    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        spinTimer = spinDuration;
    }

    #endregion

    #region 弹射模式

    /// <summary>
    /// 飞剑弹射逻辑
    /// </summary>
    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {

                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>(),.8f);

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    /// <summary>
    /// 寻找命中目标附近的可弹射目标
    /// </summary>
    /// <param name="collision"></param>
    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// 触发器，与命中物体交互
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;


        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy,1f);
        }


        SetupTargetsForBounce(collision);

        StuckInto(collision);
    }

    /// <summary>
    /// 飞剑技能伤害函数
    /// </summary>
    /// <param name="enemy"></param>
    private void SwordSkillDamage(Enemy enemy,float value)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        player.stats.DoDamage(enemyStats,value);

        if (player.skill.sword.timeStopUnlocked)
             enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.vulnerableUnlocked)
            enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

         if (equipedAmulet != null)
             equipedAmulet.Effect(enemy.transform);
    }


    /// <summary>
    /// 判断飞剑是否处于穿刺状态，若是，先穿过指定数量的敌人再执行
    /// 飞剑命中敌人或地面时成为目标物体的子物体模拟插入，此时停止旋转
    /// </summary>
    /// <param name="collision"></param>
    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }


        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
            return;


        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
