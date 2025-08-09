using System;

/// <summary>
/// 序列化接口，用于物品的仓库处理
/// </summary>
[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;
    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
