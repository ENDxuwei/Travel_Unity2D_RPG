using UnityEngine.EventSystems;

/// <summary>
/// 装备制作系统UI ，显示可制作的装备并处理点击交互
/// </summary>
public class UI_CraftSlot : UI_ItemSlot
{
    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 将装备数据显示在制作槽中
    /// </summary>
    /// <param name="_data"></param>
    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        item.data = _data;

        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * .7f;
        else
            itemText.fontSize = 24;
    }

    /// <summary>
    /// 点击制作槽时，调用CraftWindow，显示装备详情和制作材料
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
