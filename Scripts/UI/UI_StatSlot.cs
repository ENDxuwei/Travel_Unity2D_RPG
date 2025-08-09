using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 玩家属性UI显示
/// </summary>
public class UI_StatSlot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    /// <summary>
    /// 规范化游戏对象名称，并同步属性名称到 UI 文本
    /// </summary>
    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if(statNameText != null)
            statNameText.text = statName;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    /// <summary>
    /// 更新UI，计算加成
    /// </summary>
    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if(playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

            if (statType == StatType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + (playerStats.strength.GetValue()) * 0.2f).ToString();

            if(statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.GetValue() + (playerStats.agility.GetValue()) * 0.1f).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + Mathf.RoundToInt(playerStats.agility.GetValue()) * 0.15).ToString();

            if (statType == StatType.mana)
                statValueText.text = playerStats.GetMaxManaValue().ToString();
        }
    }

    /// <summary>
    /// 鼠标滑入显示属性描述
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }

    /// <summary>
    /// 鼠标划出关闭描述
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}
