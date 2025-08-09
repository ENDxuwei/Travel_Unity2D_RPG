using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 镜像技能系统
/// </summary>
public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private int facingDir = 1;

    private bool canDuplicateClone;
    private float chanceToDuplicate;

    [Space]
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float clostestEnemyCheckRadius = 25;
    [SerializeField] private Transform closestEnemy;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        StartCoroutine(FaceClosestTarget());
    }

    /// <summary>
    /// 镜像生命周期控制
    /// </summary>
    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// 镜像技能构造函数
    /// </summary>
    /// <param name="_newTransform"></param>
    /// <param name="_cloneDuration"></param>
    /// <param name="_canAttack"></param>
    /// <param name="_offset"></param>
    /// <param name="_closestEnemy"></param>
    /// <param name="_canDuplicate"></param>
    /// <param name="_chanceToDuplicate"></param>
    /// <param name="_player"></param>
    /// <param name="_attackMultiplier"></param>
    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, bool _canDuplicate, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));

        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
    }

    /// <summary>
    /// 动画事件触发镜像消失流程
    /// </summary>
    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    /// <summary>
    /// 攻击动画的关键帧事件调用，执行攻击检测和伤害计算
    /// </summary>
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                //镜像伤害
                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                //镜像可以附带攻击特效
                if (player.skill.clone.canApplyOnHitEffect)
                 {
                     ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                     if (weaponData != null)
                         weaponData.Effect(hit.transform);
                 }

                //多重镜像
                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 镜像攻击时面向最近的敌人
    /// </summary>
    private IEnumerator FaceClosestTarget()
    {
        yield return null;

        FindClosestEnemy();

        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    protected void FindClosestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25,whatIsEnemy);

        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
           
            float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = hit.transform;
            }

            
        }

    }
}
