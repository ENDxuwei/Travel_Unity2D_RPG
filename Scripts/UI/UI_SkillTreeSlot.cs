using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 技能树系统UI，负责管理单个技能的解锁逻辑、UI显示、鼠标交互以及数据持久化
/// </summary>
public class UI_SkillTreeSlot : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler ,ISaveManager
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [SerializeField] private string skillNameEn;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;


    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;
    [SerializeField] private UI_SkillTreeSlot[] needThisUnlocked;

    /// <summary>
    /// 自动命名
    /// </summary>
    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillNameEn;
    }

    /// <summary>
    /// 绑定按钮点击事件到解锁方法
    /// </summary>
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if (unlocked)
            skillImage.color = Color.white;
    }

    /// <summary>
    /// 技能解锁逻辑
    /// </summary>
    public void UnlockSkillSlot()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SkillReset();
            return;
        }

        // 检查玩家是否有足够的钱解锁技能
        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        // 检查所有前置技能是否已解锁
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("前置未解锁" + shouldBeUnlocked[i].skillName);
                return;
            }
        }

        // 检查所有冲突技能是否保持锁定
        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("技能冲突"+ shouldBeLocked[i].skillName);
                return;
            }
        }

        //解锁技能
        unlocked = true;
        skillImage.color = Color.white;
    }

    /// <summary>
    /// 取消技能选择
    /// </summary>
    public void SkillReset()
    {
        // 检查后续技能是否保持锁定
        for (int i = 0; i < needThisUnlocked.Length; i++)
        {
            if (needThisUnlocked[i].unlocked == true)
            {
                Debug.Log("此技能为其他技能前置" + needThisUnlocked[i].skillName);
                return;
            }
        }

        unlocked = false;
        skillImage.color = lockedSkillColor;
    }

    /// <summary>
    /// 鼠标悬停，显示提示框
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription,skillName,skillCost);
    }

    /// <summary>
    /// 鼠标离开，隐藏提示框
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    /// <summary>
    /// 技能树数据加载
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    /// <summary>
    /// 技能树数据保存，存储技能解锁信息
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
            _data.skillTree.Add(skillName, unlocked);
    }
}
