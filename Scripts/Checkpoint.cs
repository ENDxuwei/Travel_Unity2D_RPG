using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检查点功能，用于记录玩家进度，作为后续重生或继续游戏的起点
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activationStatus;

    /// <summary>
    /// 初始化动画组件
    /// </summary>
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 编辑器调用GenerateId生成全局唯一的GUID
    /// </summary>
    [ContextMenu("生成检查点ID")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 触发检测
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    /// <summary>
    /// 激活处理
    /// </summary>
    public void ActivateCheckpoint()
    { 
        if (activationStatus == false)
            AudioManager.instance.PlaySFX(5, transform);

        activationStatus = true;
        anim.SetBool("active", true);
    }
}
