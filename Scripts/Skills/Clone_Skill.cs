using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 镜像技能系统管理
/// </summary>
public class Clone_Skill : Skill
{


    [Header("镜像设置")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("镜像攻击")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("镜像特效")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("多重镜像")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    
    [Header("镜像水晶")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInseadOfClone;

    /// <summary>
    /// 初始化，根据技能树进度改变技能类型
    /// </summary>
    protected override void Start()
    {
        base.Start();


        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region 技能解锁
    /// <summary>
    /// 判断技能是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }

    /// <summary>
    /// 镜像攻击
    /// </summary>
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
        else
            canAttack = false;
    }

    /// <summary>
    /// 镜像特效
    /// </summary>
    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
        else
            canApplyOnHitEffect = false;
    }

    /// <summary>
    /// 多重镜像
    /// </summary>
    private void UnlockMultiClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
        else
            canDuplicateClone = false;
    }

    /// <summary>
    /// 镜像水晶
    /// </summary>
    private void UnlockCrystalInstead()
    {
        if (crystalInseadUnlockButton.unlocked)
        {
            crystalInseadOfClone = true;
        }
        else
            crystalInseadOfClone = false;
    }


    #endregion

    /// <summary>
    /// 创建镜像
    /// </summary>
    /// <param name="_clonePosition"></param>
    /// <param name="_offset"></param>
    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration, canAttack, _offset, canDuplicateClone, chanceToDuplicate, player, attackMultiplier);
    }

    /// <summary>
    /// 延迟召唤镜像
    /// </summary>
    /// <param name="_enemyTransform"></param>
    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    /// <summary>
    /// 协程，使召唤镜像时延迟一段时间
    /// </summary>
    /// <param name="_trasnform"></param>
    /// <param name="_offset"></param>
    /// <returns></returns>
    private IEnumerator CloneDelayCorotine(Transform _trasnform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_trasnform, _offset);
    }
}
