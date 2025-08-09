using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品生成与拾取系统
/// </summary>
public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    
    /// <summary>
    /// 物品出现时赋值名称和图标
    /// </summary>
    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    /// <summary>
    /// 物品构造函数
    /// </summary>
    /// <param name="_itemData"></param>
    /// <param name="_velocity"></param>
    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }

    /// <summary>
    /// 捡起物品放入仓库
    /// </summary>
    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("Inventory is full",Color.white);
            return;
        }

        AudioManager.instance.PlaySFX(18, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
