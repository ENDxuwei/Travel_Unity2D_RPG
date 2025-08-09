using UnityEngine;

/// <summary>
/// Buff效果，提升属性
/// </summary>
[CreateAssetMenu(fileName = "Buff", menuName = "Data/Item effect/Buff")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        stats.IncreaseStatBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }
}
