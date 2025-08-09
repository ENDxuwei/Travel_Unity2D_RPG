using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水晶技能系统，控制水晶技能的行为逻辑
/// </summary>
public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();
    private Player player;

    private float crystalExistTimer;


    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    /// <summary>
    /// 水晶技能构造函数
    /// </summary>
    /// <param name="_crystalDuration"></param>
    /// <param name="_canExplode"></param>
    /// <param name="_canMove"></param>
    /// <param name="_moveSpeed"></param>
    /// <param name="_closestTarget"></param>
    /// <param name="_player"></param>
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget, Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    /// <summary>
    /// 随机选择范围内的一个敌人作为目标
    /// </summary>
    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    /// <summary>
    /// 控制水晶的生命周期、移动和生长逻辑
    /// </summary>
    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        //如果可以移动，则优先寻找并追踪附近的敌人，不爆炸
        if (canMove)
        {
            if (closestTarget == null)
                return;

            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        //水晶扩大，模拟爆炸效果
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 由爆炸动画的关键帧事件调用，执行爆炸伤害检测和效果应用
    /// </summary>
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {

                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>(),1f);

                //发动护身符效果
                 ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                 if (equipedAmulet != null)
                     equipedAmulet.Effect(hit.transform);
            }
        }
    }

    /// <summary>
    /// 处理水晶的终结逻辑（爆炸或直接销毁）
    /// </summary>
    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    /// <summary>
    /// 销毁水晶
    /// </summary>
    public void SelfDestroy() => Destroy(gameObject);
}
