using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品掉落系统，生成随机掉落物品
/// </summary>
public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    /// <summary>
    /// 物品掉落逻辑
    /// </summary>
    public virtual void GenerateDrop()
    {
        if(possibleDrop.Length == 0)
        {
            Debug.Log("没有可以掉落的物品");
            return;
        }

        foreach(ItemData item in possibleDrop)
        {
            if(item != null && Random.Range(0,100) < item.dropChance)
            {
                dropList.Add(item);
            }
        }

        for(int i = 0; i < possibleItemDrop; i++)
        {
            if(dropList.Count > 0)
            {
                int randomIndex = Random.Range(0, dropList.Count);
                ItemData itemToDrop = dropList[randomIndex];

                DropItem(itemToDrop);
                dropList.Remove(itemToDrop);
            }
        }
    }


    /// <summary>
    /// 掉落物品生成
    /// </summary>
    /// <param name="_itemData"></param>
    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));


        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
