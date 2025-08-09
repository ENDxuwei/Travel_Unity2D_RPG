using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性类
/// </summary>
[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    /// <summary>
    /// 获取属性值
    /// </summary>
    /// <returns></returns>
    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    /// <summary>
    /// 设定属性为初始默认值
    /// </summary>
    /// <param name="_value"></param>
    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    /// <summary>
    /// 设定属性增加修改值
    /// </summary>
    /// <param name="_modifier"></param>
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    /// <summary>
    /// 移除属性修改值
    /// </summary>
    /// <param name="_modifier"></param>
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
