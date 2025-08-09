using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储游戏中的各种持久化数据
/// 可序列化特性，数据可以被保存到文件或从文件加载
/// </summary>
[System.Serializable]
public class GameData
{
    public int level;
    public int currentExp;
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;


    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSettings;

    /// <summary>
    /// 游戏数据
    /// </summary>
    public GameData()
    {
        this.level = 0;
        this.currentExp = 0;
        this.currency = 0;

        //记录魂的掉落位置和数量
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        //记录玩家技能树，装备栏和背包
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        //记录各个检查点是否已激活，寻找最近激活的检查点ID
        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();

        //存储各个音频通道的音量设置
        volumeSettings = new SerializableDictionary<string, float>();
    }
}
