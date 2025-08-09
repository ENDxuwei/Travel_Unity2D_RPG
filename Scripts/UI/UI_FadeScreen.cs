using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 淡入淡出屏幕效果
/// </summary>
public class UI_FadeScreen : MonoBehaviour
{
    private Animator anim;

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 淡出
    /// </summary>
    public void FadeOut() => anim.SetTrigger("fadeOut");

    /// <summary>
    /// 淡入
    /// </summary>
    public void FadeIn() => anim.SetTrigger("fadeIn");
}
