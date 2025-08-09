using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 混乱法球效果控制器
/// </summary>
public class IceAndFire_Controller : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoMagicalDamage(enemyTarget, 1f);
        }
    }
}
