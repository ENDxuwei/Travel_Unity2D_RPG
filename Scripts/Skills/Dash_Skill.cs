using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 冲刺技能
/// </summary>
public class Dash_Skill : Skill
{
    [Header("冲刺")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked { get; private set; }

    [Header("冲刺开始时创建镜像")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked { get; private set; }

    [Header("到达时创建镜像")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    public bool cloneOnArrivalUnlocked { get; private set; }


    /// <summary>
    /// 冲刺技能使用效果
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();
    }

    /// <summary>
    /// 初始化，根据技能书进度控制技能类型
    /// </summary>
    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    #region 技能解锁
    /// <summary>
    /// 检查技能是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }

    /// <summary>
    /// 冲刺
    /// </summary>
    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
            dashUnlocked = true;
        else
            dashUnlocked = false;
    }

    /// <summary>
    /// 冲刺时召唤镜像
    /// </summary>
    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
            cloneOnDashUnlocked = true;
        else
            cloneOnDashUnlocked = false;
    }

    /// <summary>
    /// 冲刺结束时召唤镜像
    /// </summary>
    private void UnlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
            cloneOnArrivalUnlocked = true;
        else
            cloneOnArrivalUnlocked = false;
    }
    #endregion


    /// <summary>
    /// 玩家冲刺时制造一个镜像,留在原地，可以攻击
    /// </summary>
    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    /// <summary>
    /// 冲刺结束时创建一个镜像，可以攻击
    /// </summary>
    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
}
