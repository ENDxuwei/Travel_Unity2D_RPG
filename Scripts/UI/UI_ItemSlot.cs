using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

/// <summary>
/// 物品槽UI，管理单个物品槽的 UI 表现和用户交互
/// </summary>
public class UI_ItemSlot : MonoBehaviour , IPointerDownHandler ,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;
    public InventoryItem item;

    /// <summary>
    /// 初始化UI组件
    /// </summary>
    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    /// <summary>
    /// 更新物品槽显示
    /// </summary>
    /// <param name="_newItem"></param>
    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.itemIcon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    /// <summary>
    /// 完全清空物品槽的显示和数据，在物品被移除或移动时调用
    /// </summary>
    public void CleanUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    /// <summary>
    /// 点击物品时，关闭提示框，若为装备则进行装备判定。按下左ctrl可以删除物品
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);

    }

    /// <summary>
    /// 鼠标进入装备时显示详细信息提示框
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    /// <summary>
    /// 鼠标离开隐藏提示框
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip();
    }
}
