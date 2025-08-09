using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家进入或离开特定区域时，控制音效的播放与停止
/// </summary>
public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;

    /// <summary>
    /// 当玩家进入当前物体的触发器时调用
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.PlaySFX(areaSoundIndex,null);
    }

    /// <summary>
    /// 当玩家离开当前物体的触发器时调用
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.StopSFXWithTime(areaSoundIndex);
    }
}
