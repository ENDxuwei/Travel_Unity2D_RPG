using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雷霆一击效果
/// </summary>
[CreateAssetMenu(fileName = "雷霆一击", menuName = "Data/Item effect/雷霆一击")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);
        Destroy(newThunderStrike, 1);
    }
}
