using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 动态更新制作窗口的UI显示，物品信息、所需材料以及绑定制作按钮
/// </summary>
public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    /// <summary>
    /// 根据传入的装备数据，更新制作窗口的所有UI元素
    /// </summary>
    /// <param name="_data"></param>
    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        //清除制作按钮的所有旧监听，防止多次点击时重复执行逻辑
        craftButton.onClick.RemoveAllListeners();

        //重置材料槽UI
        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        //显示当前物品所需的材料
        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            if (_data.craftingMaterials.Count > materialImage.Length)
                Debug.LogWarning("需求的材料数量多于工艺窗口中的材料槽位");


            materialImage[i].sprite = _data.craftingMaterials[i].data.itemIcon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }


        itemIcon.sprite = _data.itemIcon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        //点击逻辑，调用CanCraft方法，传入待制作物品数据和所需材料
        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }
}
