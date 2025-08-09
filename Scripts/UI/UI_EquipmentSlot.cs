using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 装备槽UI,处理装备槽的点击交互逻辑，允许玩家卸下已装备的物品
/// </summary>
public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    /// <summary>
    /// 命名GameObject
    /// </summary>
    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    /// <summary>
    /// 当玩家点击装备槽时，卸下装备，清空槽位，并在仓库里增加被卸下的装备
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        ui.itemToolTip.HideToolTip();

        CleanUpSlot();
    }
}
