using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 展示装备类物品的详细信息
/// </summary>
public class UI_ItemTooltip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private int defaultFontSize = 32;

    /// <summary>
    /// 根据传入的装备数据显示详细信息
    /// </summary>
    /// <param name="item"></param>
    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null)
            return;

        itemNameText.text = item.itemName;
        itemNameText.color = item.ColorToRarity(item.equipmentRarity);
        itemTypeText.text = item.TypeToString(item.equipmentType);
        itemDescription.text = item.GetDescription();

        AdjustFontSize(itemNameText);
        AdjustPosition();

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏 Tooltip 并重置字体大小
    /// </summary>
    public void HideToolTip() 
    {
        itemNameText.fontSize = defaultFontSize;
        gameObject.SetActive(false);
    }

}
