using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家物品掉落系统
/// </summary>
public class PlayerItemDrop : ItemDrop
{
    [Header("玩家掉落")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    /// <summary>
    /// 物品掉落逻辑
    /// </summary>
    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();

        //装备掉落逻辑
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }
        //更新背包
        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment); 
        }


        //材料掉落逻辑
        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }
        //更新背包
        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }
    }
}
