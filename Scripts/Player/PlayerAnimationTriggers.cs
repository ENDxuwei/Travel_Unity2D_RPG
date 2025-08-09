using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画触发器函数
/// </summary>
public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    /// <summary>
    /// 动画结束触发器
    /// </summary>
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    /// <summary>
    /// 攻击事件触发器
    /// </summary>
    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                {
                    player.stats.DoDamage(_target,1f);
                }

                //获取武器效果
                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                 if (weaponData != null)
                     weaponData.Effect(_target.transform);
            }
        }
    }

    /// <summary>
    /// 创建飞剑触发器
    /// </summary>
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
