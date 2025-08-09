using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可序列化的字典类
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> , ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    /// <summary>
    /// 序列化前的转换
    /// </summary>
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        // 遍历字典，将键值对分别存入两个列表
        foreach (KeyValuePair<TKey,TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    /// <summary>
    /// 反序列化后的还原
    /// </summary>
    public void OnAfterDeserialize()
    {
        this.Clear();

        // 校验键值列表长度是否一致
        if (keys.Count != values.Count)
        {
            Debug.Log("Keys count is not equal to values count");
        }

        // 遍历列表，将键值对重新添加到字典
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

}
