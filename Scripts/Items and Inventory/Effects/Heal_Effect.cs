using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 治疗效果
/// </summary>
[CreateAssetMenu(fileName = "治疗", menuName = "Data/Item effect/治疗")]
public class Heal_Effect : ItemEffect
{
    [Range(0f,1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt( playerStats.GetMaxHealthValue() * healPercent);

        playerStats.IncreaseHealthBy(healAmount);
    }
}
