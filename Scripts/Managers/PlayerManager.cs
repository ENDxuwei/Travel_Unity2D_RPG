using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 玩家管理器
/// </summary>
public class PlayerManager : MonoBehaviour , ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        //单例模式
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency = currency - _price;
        return true;
    }

    public int GetCurrency() => currency;

    /// <summary>
    /// 玩家数据加载
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data)
    {
        PlayerStats playerStats = instance.player.GetComponent<PlayerStats>();
        this.currency = _data.currency;
        playerStats.exp = _data.currentExp;
        playerStats.level = _data.level;
        playerStats.Modify();
        playerStats.ApplyLevelModifiers();
        playerStats.currentHealth = playerStats.maxHealth.GetValue() + 5 * playerStats.strength.GetValue();
        playerStats.currentMana = playerStats.maxMana.GetValue() + 5 * playerStats.intelligence.GetValue();
    }

    /// <summary>
    /// 玩家数据存储
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        PlayerStats playerStats = instance.player.GetComponent<PlayerStats>();
        _data.level = playerStats.level;
        _data.currentExp = playerStats.exp;
        _data.currency = this.currency;
    }
}
