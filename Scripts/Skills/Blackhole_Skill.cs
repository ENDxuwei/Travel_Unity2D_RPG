using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 黑洞技能类
/// </summary>
public class Blackhole_Skill : Skill
{

    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackholeUnlocked;// { get; private set; }
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;


    Blackhole_Skill_Controller currentBlackhole;

    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 黑洞技能解锁
    /// </summary>
    private void UnlockBlackhole()
    {
        if (blackHoleUnlockButton.unlocked)
            blackholeUnlocked = true;
        else
            blackholeUnlocked = false;

    }

    /// <summary>
    /// 检测黑洞技能是否可用
    /// </summary>
    /// <returns></returns>
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    /// <summary>
    /// 黑洞技能使用效果
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();



        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration);

        AudioManager.instance.PlaySFX(6, player.transform);
    }


    /// <summary>
    /// 黑洞技能完成，清除黑洞
    /// </summary>
    /// <returns></returns>
    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false;


        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取黑洞大小的一半，作为寻敌半径，防止技能攻击到范围外的敌人
    /// </summary>
    /// <returns></returns>
    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

    /// <summary>
    /// 检查是否锁定
    /// </summary>
    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockBlackhole();
    }
}
