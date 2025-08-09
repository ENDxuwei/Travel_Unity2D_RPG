using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏存档系统中的核心接口，用于规范所有需要参与数据保存和加载的系统的行为
/// </summary>
public interface ISaveManager 
{
    void LoadData(GameData _data);
    void SaveData(ref GameData _data);
}
