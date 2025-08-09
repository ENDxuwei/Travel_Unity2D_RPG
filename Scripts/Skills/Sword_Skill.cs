using System;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

/// <summary>
/// 投掷飞剑技能
/// </summary>
public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    #region 模式
    [Header("弹射模式设置")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("穿刺模式设置")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("旋转模式设置")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    #endregion

    [Header("技能设置")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("被动技能")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked { get; private set; }

    private Vector2 finalDir;

    [Header("瞄准辅助线点")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetwenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    /// <summary>
    /// 初始化，根据技能树调整技能
    /// </summary>
    protected override void Start()
    {
        base.Start();

        GenereateDots();
        SetupGraivty();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnurable);

    }

    /// <summary>
    /// 瞄准鼠标所在位置
    /// </summary>
    protected override void Update()
    {
        //确定瞄准方向
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        //确定瞄准线点的位置
        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }
        }
    }

    /// <summary>
    /// 设置重力
    /// </summary>
    private void SetupGraivty()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordGravity = bounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
        }
    }

    /// <summary>
    /// 创造飞剑物体
    /// </summary>
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        //根据飞剑技能调整飞剑类型
        switch (swordType)
        {
            case SwordType.Regular:
                break;
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
                break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                break;
        }


        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        //分配新的飞剑
        player.AssignNewSword(newSword);

        DotsActive(false);
    }


    #region 技能解锁

    /// <summary>
    /// 检查技能是否解锁
    /// </summary>
    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockSpinSword();
        UnlockPierceSword();
        UnlockTimeStop();
        UnlockVulnurable();
    }

    /// <summary>
    /// 投掷飞剑时时间暂停技能
    /// </summary>
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
        else
            timeStopUnlocked = false;
    }

    /// <summary>
    /// 飞剑脆弱技能
    /// </summary>
    private void UnlockVulnurable()
    {
        if (vulnerableUnlockButton.unlocked)
            vulnerableUnlocked = true;
        else
            vulnerableUnlocked = false;
    }

    /// <summary>
    /// 飞剑技能
    /// </summary>
    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
        else
            swordUnlocked = false;
    }

    /// <summary>
    /// 弹射技能
    /// </summary>
    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
        else
            swordType = SwordType.Regular;
    }

    /// <summary>
    /// 穿刺技能
    /// </summary>
    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
        else
            swordType = SwordType.Regular;
    }

    /// <summary>
    /// 旋转技能
    /// </summary>
    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
        else
            swordType = SwordType.Regular;
    }



    #endregion

    #region 瞄准

    /// <summary>
    /// 基于鼠标位置获取瞄准方向
    /// </summary>
    /// <returns></returns>
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    /// <summary>
    /// 管理瞄准线点开关
    /// </summary>
    /// <param name="_isActive"></param>
    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    /// <summary>
    /// 基于瞄准方向生成点
    /// </summary>
    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    /// <summary>
    /// 确定瞄准线点的位置
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion
}
