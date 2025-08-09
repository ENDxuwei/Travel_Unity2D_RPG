using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理物品拾取的触发器
/// </summary>
public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    /// <summary>
    /// 当玩家与物品发生碰撞，将物品放入仓库
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (collision.GetComponent<CharacterStats>().isDead)
                return;

            Debug.Log("捡到了物品 ");
            myItemObject.PickupItem();
        }
    }
}
