using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 背包管理
/// </summary>
public class Inventory : MonoBehaviour , ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("背包UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equpmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("物品冷却")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown { get; private set; }
    public float staffCooldown { get; private set; }
    public float pendantCooldown { get; private set; }
    public float fibreCooldown { get; private set; }
    public float armorCooldown { get; private set; }

    [Header("数据存储库")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;
    
    /// <summary>
    /// 单例
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 初始化UI容器和UI槽位
    /// </summary>
    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equpmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        //添加初始物品
        Invoke("AddStartingItems", 0.5f);
    }

    /// <summary>
    /// 添加初始物品
    /// </summary>
    private void AddStartingItems()
    {
        //加载存档装备，直接穿戴
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }
        
        //加载存档物品，进入背包
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }

            return;
        }

        //存档为空时，加载初始物品进入背包
        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    #region 装备

    /// <summary>
    /// 穿戴装备
    /// </summary>
    /// <param name="_item"></param>
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        // 检查同类型装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        // 若已存在同类型装备则卸下旧装备并添加到背包
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        // 添加新装备到装备列表并应用属性修改
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        // 从背包移除该物品
        RemoveItem(_item);

        //更新UI
        UpdateSlotUI();
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    /// <param name="itemToRemove"></param>
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        // 从装备列表移除物品并移除属性修改
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    #endregion

    /// <summary>
    /// 更新装备槽、装备背包槽、材料背包槽的UI显示
    /// </summary>
    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }


        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();
    }

    /// <summary>
    /// 更新角色UI中的统计信息
    /// </summary>
    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }
    #region 背包的添加与移除

    /// <summary>
    /// 向背包中添加物品，根据物品分类不同添加至不同背包
    /// </summary>
    /// <param name="_item"></param>
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    /// <summary>
    /// 添加至材料背包，如果存在相同物品则进行堆叠，否则新建一个槽位储存
    /// </summary>
    /// <param name="_item"></param>
    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    /// <summary>
    /// 添加至装备背包，如果存在相同物品则进行堆叠，否则新建一个槽位储存
    /// </summary>
    /// <param name="_item"></param>
    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    /// <summary>
    /// 从仓库中移除物品，如果数量为零，删除槽位，反之则减少堆叠数
    /// </summary>
    /// <param name="_item"></param>
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    /// <summary>
    /// 检测是否还有足够空槽位存放物品
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            return false;
        }

        return true;
    }

    #endregion

    /// <summary>
    /// 检查能否进行制作
    /// </summary>
    /// <param name="_itemToCraft"></param>
    /// <param name="_requiredMaterials"></param>
    /// <returns></returns>
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        foreach(var requiredItem in _requiredMaterials)
        {
            if(stashDictionary.TryGetValue(requiredItem.data, out InventoryItem stashItem))
            {
                if(stashItem.stackSize < requiredItem.stackSize)
                {
                    Debug.Log("材料不足" + requiredItem.data.name);
                    return false;
                }
            }
            else
            {
                Debug.Log("材料未找到" + requiredItem.data.name);
                return false;
            }
        }

        foreach(var requiredMaterial in _requiredMaterials)
        {
            for (int i = 0; i < requiredMaterial.stackSize; i++)
            {
                RemoveItem(requiredMaterial.data);
            }
        }

        AddItem(_itemToCraft);
        Debug.Log("制作成功" + _itemToCraft.name);

        return true;
    }

    /// <summary>
    /// 获取装备列表
    /// </summary>
    /// <returns></returns>
    public List<InventoryItem> GetEquipmentList() => equipment;

    /// <summary>
    /// 获取材料列表
    /// </summary>
    /// <returns></returns>
    public List<InventoryItem> GetStashList() => stash;

    /// <summary>
    /// 从装备字典中获取特定类型的装备数据
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    /// <summary>
    /// 使用药水
    /// </summary>
    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("药水冷却");
    }

    /// <summary>
    /// 盔甲被动技能
    /// </summary>
    /// <returns></returns>
    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("护甲技能冷却");
        return false;
    }

    /// <summary>
    /// 从存档数据中获取物品和装备状态
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            // 物品栏数据恢复
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        // 装备数据恢复
        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }

    }

    /// <summary>
    /// 将当前物品和装备状态写入存档数据
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }


    /// <summary>
    /// 物品信息的存储，仅在编辑器环境中使用
    /// </summary>
#if UNITY_EDITOR
    [ContextMenu("填充物品数据库")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    /// <summary>
    /// 物品数据库获取逻辑，仅在编辑器环境中使用
    /// </summary>
    /// <returns></returns>
    private List<ItemData> GetItemDataBase()
    {
        // 查找指定目录下的所有资产
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        // 加载路径对应的ItemData资产
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
    

}
