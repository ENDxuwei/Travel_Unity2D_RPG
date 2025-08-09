using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 招架技能类
/// </summary>
public class Parry_Skill : Skill
{

    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0f,1f)]
    [SerializeField] private float restoreHealthPerentage;
    public bool restoreUnlocked { get; private set; }

    [Header("反击镜像攻击")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    /// <summary>
    /// 招架技能使用效果
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();


        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * restoreHealthPerentage);
            player.stats.IncreaseHealthBy(restoreAmount);
        }

    }

    /// <summary>
    /// 初始化，根据技能树进度控制技能类型
    /// </summary>
    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    #region 技能解锁
    /// <summary>
    /// 检查技能是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryRestore();
        UnlockParryWithMirage();
    }

    /// <summary>
    /// 招架
    /// </summary>
    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
        else
            parryUnlocked = false;
    }

    /// <summary>
    /// 招架回血
    /// </summary>
    private void UnlockParryRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
        else
            restoreUnlocked = false;
    }

    /// <summary>
    /// 招架反击
    /// </summary>
    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
        else
            parryWithMirageUnlocked = false;
    }
    #endregion

    /// <summary>
    /// 反击时在敌人位置创建镜像攻击
    /// </summary>
    /// <param name="_respawnTransform"></param>
    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }

}
