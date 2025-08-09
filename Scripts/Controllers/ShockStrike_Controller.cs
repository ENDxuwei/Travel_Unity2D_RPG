using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 感电雷击控制器
/// </summary>
public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    /// <summary>
    /// 初始化组件
    /// </summary>
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_targetStats"></param>
    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    /// <summary>
    /// 移动与攻击逻辑
    /// </summary>
    void Update()
    {
        if(!targetStats)
            return;


        if (triggered)
            return;

        //// 向目标移动并调整朝向
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        //// 到达目标附近时触发攻击
        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation = Quaternion.identity;

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            Invoke("DamageAndSelfDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    /// <summary>
    /// 造成伤害并销毁
    /// </summary>
    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
    
}
