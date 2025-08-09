using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 用以调整工具提示位置偏移量和字体大小
/// </summary>
public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit => Screen.width / 2f;
    [SerializeField] private float yLimit => Screen.height / 2f;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    /// <summary>
    /// 位置调整
    /// </summary>
    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        float newXoffset = 0;
        float newYoffset = 0;

        // 根据鼠标位置决定偏移方向
        if (mousePosition.x > xLimit)
            newXoffset = -xOffset;
        else
            newXoffset = xOffset;

        if (mousePosition.y > yLimit)
            newYoffset = -yOffset;
        else
            newYoffset = yOffset;

        transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);
    }

    /// <summary>
    /// 字体大小调整
    /// </summary>
    /// <param name="_text"></param>
    public void AdjustFontSize(TextMeshProUGUI _text)
    {
        if (_text.text.Length > 12)
            _text.fontSize = _text.fontSize * .8f;
    }
}
