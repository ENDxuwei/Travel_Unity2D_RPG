using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 水晶技能类
/// </summary>
public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("水晶镜像")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("水晶")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("水晶爆炸")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private float explisoveCooldown;
    [SerializeField] private bool canExplode;


    [Header("水晶移动")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;


    [Header("多水晶堆叠")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWondow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    /// <summary>
    /// 初始化，根据技能树进度控制技能类型
    /// </summary>
    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);

    }

    #region 技能解锁
    /// <summary>
    /// 检测技能是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }

    /// <summary>
    /// 水晶
    /// </summary>
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
        else
            crystalUnlocked = false;
    }

    /// <summary>
    /// 水晶镜像
    /// </summary>
    private void UnlockCrystalMirage()
    {
        if (unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
        else
            cloneInsteadOfCrystal = false;
    }

    /// <summary>
    /// 水晶爆炸
    /// </summary>
    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
        {
            canExplode = true;
            cooldown = explisoveCooldown;
        }
        else
            canExplode = false;
    }

    /// <summary>
    /// 水晶移动
    /// </summary>
    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
        else
            canMoveToEnemy = false;
    }

    /// <summary>
    /// 多重水晶
    /// </summary>
    private void UnlockMultiStack()
    {
        if (unlockMovingCrystalButton.unlocked)
            canUseMultiStacks = true;
        else
            canUseMultiStacks = false;
    }

    #endregion     

    /// <summary>
    /// 水晶技能使用效果
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    /// <summary>
    /// 创造水晶
    /// </summary>
    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }
    
    /// <summary>
    /// 随机寻敌
    /// </summary>
    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    /// <summary>
    /// 检测是否可以使用多个水晶，填充水晶列表
    /// </summary>
    /// <returns></returns>
    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWondow);

                cooldown = 0;
                cooldownTimer = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 填充多水晶列表
    /// </summary>
    private void RefilCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    /// <summary>
    /// 重置多水晶技能，使技能只要使用即使未完成也会自动装填
    /// </summary>
    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;

        RefilCrystal();
    }
}
