using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 弹出文字效果
/// </summary>
public class PopUpTextFx : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float disappearanceSpeed;
    [SerializeField] private float colordisappearanceSpeed;

    [SerializeField] private float lifeTime;


    private float textTimer;

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    /// <summary>
    /// 生命周期管理
    /// </summary>
    void Update()
    {
        // 文字持续向上移动
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);
        textTimer -= Time.deltaTime;

        // 当计时器归零时，开始淡化消失
        if (textTimer < 0)
        {
            float alpha = myText.color.a - colordisappearanceSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            // 当透明度低于阈值时，加快移动速度
            if (myText.color.a < 50)
                speed = disappearanceSpeed;

            // 完全透明后销毁对象
            if (myText.color.a <= 0)
                Destroy(gameObject);
        }
    }
}
