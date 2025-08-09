using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 闪避技能，负责技能的解锁、属性加成及镜像创建
/// </summary>
public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked;

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodge;
    public bool dodgeMirageUnlocked;

    /// <summary>
    /// 初始化，根据技能树进度控制技能形态
    /// </summary>
    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodge.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    /// <summary>
    /// 检查是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    /// <summary>
    /// 闪避解锁
    /// </summary>
    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
        else
        { 
            dodgeUnlocked = false;
        }
    }

    /// <summary>
    /// 闪避创造镜像解锁
    /// </summary>
    private void UnlockMirageDodge()
    {
        if (unlockMirageDodge.unlocked)
            dodgeMirageUnlocked = true;
        else
            dodgeMirageUnlocked = false;
    }

    /// <summary>
    /// 闪避时创造镜像回击
    /// </summary>
    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir,0));
    }
}
