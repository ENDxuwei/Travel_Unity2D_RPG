using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能类
/// </summary>
public class Skill : MonoBehaviour
{
    public float cooldown;
    public float cooldownTimer;

    protected Player player;


    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        Invoke("CheckUnlock", 0.5f);
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }


    protected virtual void CheckUnlock()
    {

    }

    /// <summary>
    /// 检测技能是否可用
    /// </summary>
    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        player.fx.CreatePopUpText("Cooldown",Color.white);
        return false;
    }

    /// <summary>
    /// 技能使用效果
    /// </summary>
    public virtual void UseSkill()
    {
        
    }

    /// <summary>
    /// 释放技能时寻找最近的敌人
    /// </summary>
    /// <param name="_checkTransform"></param>
    /// <returns></returns>
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }

            }
        }

        return closestEnemy;
    }
}
