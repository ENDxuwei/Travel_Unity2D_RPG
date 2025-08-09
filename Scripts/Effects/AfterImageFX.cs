using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 残影效果
/// </summary>
public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;

    /// <summary>
    /// 设置残影的消失速度和显示的图像
    /// </summary>
    /// <param name="_loosingSpeed"></param>
    /// <param name="_spriteImage"></param>
    public void SetupAfterImage(float _loosingSpeed, Sprite _spriteImage)
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = _spriteImage;
        colorLooseRate = _loosingSpeed;


    }

    /// <summary>
    /// 每一帧更新残影的透明度，直到完全透明后销毁物体
    /// </summary>
    private void Update()
    {
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);


        if (sr.color.a <= 0)
            Destroy(gameObject);
    }
}
