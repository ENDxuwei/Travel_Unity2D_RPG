using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 敌人动画触发器
/// </summary>
public class EnemyAnimationTriggers : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    /// <summary>
    /// 动画结束触发
    /// </summary>
    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    /// <summary>
    /// 攻击触发
    /// </summary>
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target,1f);
            }
        }
    }

    /// <summary>
    /// 特殊攻击动画触发器
    /// </summary>
    private void SpecialAttackTrigger()
    {
        enemy.AnimationSepcialAttackTrigger();
    }

    //反击触发
    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
