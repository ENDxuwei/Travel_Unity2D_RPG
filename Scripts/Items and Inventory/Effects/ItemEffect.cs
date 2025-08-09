using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 物品效果类
/// </summary>
[CreateAssetMenu(fileName ="New Item Effect",menuName ="Data/Iem Effect")]
public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("效果发动成功!");
    }
}
