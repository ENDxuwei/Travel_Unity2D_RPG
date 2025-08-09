using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理游戏内UI显示的核心类
/// </summary>
public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Slider expSlider;

    [Header("技能")]
    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;

    [Header("装备")]
    [SerializeField] private Image flaskImage;

    private SkillManager skills;

    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private float level; 

    [Header("持有灵魂")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    /// <summary>
    /// 初始化，订阅生命值变化事件，获取技能管理器单例
    /// </summary>
    void Start()
    {
        if (playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
            playerStats.onManaChanged += UpdateManaUI;
            playerStats.onExpChanged += UpdateExpUI;
        }
        skills = SkillManager.instance;
    }

    /// <summary>
    /// 检测技能按键输入，触发冷却效果，更新所有技能的冷却显示，刷新UI
    /// </summary>
    void Update()
    {
        UpdateSoulsUI();
        UpdateLevelUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackholeUnlocked)
            SetCooldownOf(blackholeImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);

        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    /// <summary>
    /// 灵魂数量UI更新
    /// </summary>
    private void UpdateSoulsUI()
    {
        // 平滑过渡动画：当当前显示值小于实际值时，逐渐增加
        if (soulsAmount < PlayerManager.instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.GetCurrency();


        currentSouls.text = ((int)soulsAmount).ToString();
    }

    /// <summary>
    /// 等级UI更新
    /// </summary>
    private void UpdateLevelUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        // 平滑过渡动画：当当前显示值小于实际值时，逐渐增加
        if (level < playerStats.level)
            level += Time.deltaTime * increaseRate;
        else
            level = playerStats.level;


        currentLevel.text = "LV \n" + ((int)level).ToString();
    }

    /// <summary>
    /// 生命值UI，仅在生命值变化时更新
    /// </summary>
    private void UpdateHealthUI()
    {
        healthSlider.maxValue = playerStats.GetMaxHealthValue();
        healthSlider.value = playerStats.currentHealth;
    }

    /// <summary>
    /// 法力值UI，仅在法力值变化时更新
    /// </summary>
    private void UpdateManaUI()
    {
        manaSlider.maxValue = playerStats.GetMaxManaValue();
        manaSlider.value = playerStats.currentMana;
    }

    /// <summary>
    /// 经验值UI，仅在经验值变化时更新
    /// </summary>
    private void UpdateExpUI()
    {
        expSlider.maxValue = playerStats.expToLevelUp.GetValue();
        expSlider.value = playerStats.exp;
    }

    /// <summary>
    /// 触发冷却（设置填充量为100%）
    /// </summary>
    /// <param name="_image"></param>
    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }
    /// <summary>
    /// 更新冷却进度
    /// </summary>
    /// <param name="_image"></param>
    /// <param name="_cooldown"></param>
    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
