using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡使者技能控制器
/// </summary>
public class DeathBringerSpell_Controller : MonoBehaviour
{

    [SerializeField] Transform check;
    [SerializeField] Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;

    /// <summary>
    /// 设置技能
    /// </summary>
    /// <param name="_stats"></param>
    public void SetupSpell(CharacterStats _stats) => myStats = _stats;

    /// <summary>
    /// 动画触发，造成魔法伤害
    /// </summary>
    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoMagicalDamage(hit.GetComponent<CharacterStats>(),1);
            }
        }
    }

    /// <summary>
    /// 绘制辅助线
    /// </summary>
    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    /// <summary>
    /// 自我销毁
    /// </summary>
    private void SelfDestory() => Destroy(gameObject);
}