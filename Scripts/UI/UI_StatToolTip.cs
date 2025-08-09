using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 展示角色属性的详细描述
/// </summary>
public class UI_StatToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI description;

    /// <summary>
    /// 根据传入的文本显示属性描述
    /// </summary>
    /// <param name="_text"></param>
    public void ShowStatToolTip( string _text)
    {
        description.text = _text;
        AdjustPosition();

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏并清空文本内容
    /// </summary>
    public void HideStatToolTip()
    {
        description.text = "";
        gameObject.SetActive(false);
    }


}
