using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 技能显示UI，显示技能的详细信息
/// </summary>
public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;

    /// <summary>
    /// 接收技能描述、技能名称、技能消耗，并分别赋值给对应的 UI 文本组件,调整提示框显示位置,激活提示框物体
    /// </summary>
    /// <param name="_skillDescprtion"></param>
    /// <param name="_skillName"></param>
    /// <param name="_price"></param>
    public void ShowToolTip(string _skillDescprtion,string _skillName,int _price)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescprtion;
        skillCost.text = "Cost: " + _price;

        AdjustPosition();

        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }
 
}
