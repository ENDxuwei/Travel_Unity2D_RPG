using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 血条UI显示
/// </summary>
public class UI_HealthBar : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private CharacterStats myStats => GetComponentInParent<CharacterStats>();
    private RectTransform myTransform;
    private Slider slider;


    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        //委托敌人翻转函数执行血条翻转
        entity.onFlipped += FlipUI;
        //委托血量变化函数执行血量UI变化
        myStats.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    /// <summary>
    /// 根据血量更新血条UI
    /// </summary>
    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void OnEnable()
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }

    /// <summary>
    /// 注销
    /// </summary>
    private void OnDisable()
    {
        if(entity != null)
            entity.onFlipped -= FlipUI;

        if(myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }

    /// <summary>
    /// 翻转血条，在敌人翻转时调用，防止出现血条来回翻转的情况
    /// </summary>
    private void FlipUI() => myTransform.Rotate(0, 180, 0);
}
