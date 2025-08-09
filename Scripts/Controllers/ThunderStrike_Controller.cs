using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雷霆一击控制器
/// </summary>
public class ThunderStrike_Controller : MonoBehaviour
{
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoLightningDamage(enemyTarget,1.5f);
        }
    }
}
