using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 制作系统列表UI，管理可制作物品的列表展示
/// </summary>
public class UI_CraftList : MonoBehaviour , IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;


    /// <summary>
    ///  初始化
    /// </summary>
    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftWindow();
    }

    /// <summary>
    /// 动态生成所有可制作物品的槽位
    /// </summary>
    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
        if (craftEquipment.Count > 0 && craftEquipment[0] != null) 
        { 
            for (int i = 0; i < craftEquipment.Count; i++)
            {
                GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
                newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
            }
        }
    }

    /// <summary>
    /// 点击刷新列表
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    /// <summary>
    /// 设置默认制作窗口
    /// </summary>
    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
}
